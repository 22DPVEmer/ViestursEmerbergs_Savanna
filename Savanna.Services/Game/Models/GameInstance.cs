using Microsoft.Extensions.Logging;
using Savanna.Common.Interfaces;
using Savanna.Common.Models;
using Savanna.GameEngine;
using System.Collections.Concurrent;
using Savanna.GameEngine.Factory;
using Savanna.Common.Configuration;

namespace Savanna.Services.Game.Models;

public class GameInstance
{
    private readonly IGameField _gameField;
    private readonly Dictionary<IGameEntity, string> _entityIds = new();
    private readonly Dictionary<IGameEntity, AnimalState> _animalStates = new();
    private readonly ILogger _logger;
    private readonly IAnimalFactory _animalFactory;
    private int _nextEntityId = 1;
    private readonly ConcurrentDictionary<string, bool> _selectedAnimals = new();

    public int Width => 20;  // Default size
    public int Height => 20; // Default size
    public int Iteration { get; private set; }
    public bool IsPaused { get; set; }

    public void SetIteration(int iteration)
    {
        _logger.LogInformation("Setting iteration to {Iteration}", iteration);
        Iteration = iteration;
    }

    public GameInstance(ILogger logger)
    {
        _logger = logger;
        try
        {
            // Get the web app's base directory
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var pluginsPath = Path.Combine(baseDir, "Plugins");
            var configPath = Path.Combine(baseDir, "config.json");
            
            _logger.LogInformation("Base directory: {BaseDir}", baseDir);
            _logger.LogInformation("Looking for plugins in: {PluginsPath}", pluginsPath);
            _logger.LogInformation("Config path: {ConfigPath}", configPath);
            
            Directory.CreateDirectory(pluginsPath);
            
            // Initialize plugin configuration
            PluginConfigurationLoader.Initialize(configPath, logger);
            
            // Log the plugin files found
            var pluginFiles = Directory.GetFiles(pluginsPath, "*.dll");
            _logger.LogInformation("Found plugin files: {Files}", string.Join(", ", pluginFiles));
            
            _animalFactory = new AnimalFactory(pluginsPath);

            // Log available animal types
            var availableTypes = _animalFactory.GetAvailableAnimalTypes();
            _logger.LogInformation("Available animal types: {Types}", 
                string.Join(", ", availableTypes));
            
            _gameField = new GameField(_animalFactory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize game instance");
            throw;
        }
    }

    public void InitializeGame()
    {
        try
        {
            _logger.LogInformation("Initializing new game");
            IsPaused = false;
            Iteration = 0;
            _entityIds.Clear();
            _animalStates.Clear();
            _selectedAnimals.Clear();
            _logger.LogInformation("Game initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize game");
            throw;
        }
    }

    public string GetEntityId(IGameEntity entity)
    {
        if (!_entityIds.TryGetValue(entity, out var id))
        {
            id = $"animal_{_nextEntityId++}";
            _entityIds[entity] = id;
        }
        return id;
    }

    public void Update()
    {
        if (!IsPaused)
        {
            _gameField.Update();
            Iteration++;
            
            // Update age for all living animals
            foreach (var animal in _gameField.Animals.Where(a => a.IsAlive))
            {
                if (_animalStates.TryGetValue(animal, out var state))
                {
                    state.Age++;
                }
            }
        }
    }

    public Dictionary<string, int> GetAnimalCounts()
    {
        return _gameField.Animals
            .Where(a => a.IsAlive)
            .GroupBy(a => a.GetType().Name)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    public IEnumerable<IGameEntity> GetAnimals()
    {
        return _gameField.Animals;
    }

    public IGameEntity? GetAnimal(string id)
    {
        return _gameField.Animals.FirstOrDefault(a => GetEntityId(a) == id);
    }
    // Main method to add animal

    public void AddAnimal(char type, Position position)
    {
        try
        {
            _logger.LogInformation("Adding animal of type {Type} at position ({X}, {Y})", type, position.X, position.Y);

            // If position is (-1, -1), generate a random position
            if (position.X == -1 && position.Y == -1)
            {
                var random = new Random();
                var maxAttempts = 10;
                bool positionFound = false;

                do
                {
                    position = new Position(
                        random.Next(0, Width),
                        random.Next(0, Height)
                    );

                    // Check if position is occupied
                    var entitiesAtPosition = _gameField.GetEntitiesAt(position).ToList();
                    positionFound = !entitiesAtPosition.Any();
                    maxAttempts--;
                } while (!positionFound && maxAttempts > 0);

                if (!positionFound)
                {
                    _logger.LogWarning("Could not find empty position for new animal");
                    return;
                }
            }

            _logger.LogInformation("Adding animal of type {Type} at position ({X}, {Y})", type, position.X, position.Y);
            _gameField.AddAnimal(type, position);
            _logger.LogInformation("Successfully added animal of type {Type} at ({X}, {Y})", type, position.X, position.Y);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add animal of type {Type}", type);
            throw;
        }
    }

    // Keep this method for backward compatibility
    public void AddAnimal(string type)
    {
        try
        {
            _logger.LogInformation("Adding animal of type {Type} using string overload", type);
            
            if (string.IsNullOrEmpty(type))
            {
                throw new ArgumentException("Animal type cannot be null or empty");
            }

            // Generate random position
            var random = new Random();
            var maxAttempts = 10;
            Position position;
            bool positionFound = false;

            do
            {
                position = new Position(
                    random.Next(0, Width),
                    random.Next(0, Height)
                );

                // Check if position is occupied
                var entitiesAtPosition = _gameField.GetEntitiesAt(position).ToList();
                positionFound = !entitiesAtPosition.Any();
                maxAttempts--;
            } while (!positionFound && maxAttempts > 0);

            if (!positionFound)
            {
                _logger.LogWarning("Could not find empty position for new animal");
                return;
            }
            
            _logger.LogInformation("Adding {Type} at position ({X}, {Y})", type, position.X, position.Y);
            
            // Map animal type to correct symbol using configuration
            char animalSymbol = type switch
            {
                "Lion" => 'L',
                "Antelope" => 'A',
                "Tiger" => 'T',
                "Zebra" => 'Z',
                _ => throw new ArgumentException($"Unknown animal type: {type}")
            };
            
            _gameField.AddAnimal(animalSymbol, position);
            
            _logger.LogInformation("Animal {Type} added successfully at ({X}, {Y})", type, position.X, position.Y);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add animal of type {Type}", type);
            throw;
        }
    }

    public void ToggleAnimalSelection(string animalId)
    {
        if (_selectedAnimals.ContainsKey(animalId))
        {
            _selectedAnimals.TryRemove(animalId, out _);
        }
        else
        {
            _selectedAnimals.TryAdd(animalId, true);
        }
    }

    public bool IsAnimalSelected(string animalId)
    {
        return _selectedAnimals.ContainsKey(animalId);
    }

    public AnimalState? GetAnimalState(IGameEntity entity)
    {
        return _animalStates.TryGetValue(entity, out var state) ? state : null;
    }

    public void Quit()
    {
        IsPaused = true;
        // Clean up resources if needed
    }

    public GameStateDto GetGameState()
    {
        var animals = GetAnimals()
            .Where(a => a.IsAlive)
            .Select(a => new AnimalDto
            {
                Id = GetEntityId(a),
                Type = a.GetType().Name,
                X = a.Position.X,
                Y = a.Position.Y,
                Health = (int)a.Health,
                IsAlive = a.IsAlive,
                IsSelected = IsAnimalSelected(GetEntityId(a))
            }).ToList();

        return new GameStateDto
        {
            Width = Width,
            Height = Height,
            Iteration = Iteration,
            Animals = animals,
            AnimalCounts = GetAnimalCounts(),
            IsPaused = IsPaused
        };
    }
} 