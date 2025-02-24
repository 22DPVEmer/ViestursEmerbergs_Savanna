using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Savanna.Common.Models;
using Savanna.Common.Interfaces;
using Savanna.Common.Plugin;
using Savanna.Infrastructure.Data;
using Savanna.Services.Game.Models;
using Savanna.Services.Interfaces;
using System.Collections.Concurrent;
using Savanna.GameEngine.Models;
using Savanna.Services.Constants;
using Savanna.Services.Exceptions;

namespace Savanna.Services.Services
{
    /// <summary>
    /// Service class that handles game logic and operations.
    /// Manages game instances, user sessions, and game state persistence.
    /// </summary>
    public class GameService : IGameService
    {
        private readonly ConcurrentDictionary<string, GameInstance> _games = new();
        private readonly ConcurrentDictionary<string, string> _userGameMap = new(); // userId -> gameId
        private readonly ILogger<GameService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly AnimalConfigurationService _animalConfig;
        private readonly PluginLoader _pluginLoader;
        private readonly object _lock = new object(); // Add lock object for thread safety

        /// <summary>
        /// Initializes a new instance of the GameService class.
        /// </summary>
        /// <param name="logger">Logger for recording service operations</param>
        /// <param name="serviceProvider">Service provider for dependency injection</param>
        /// <param name="animalConfig">Service for managing animal configurations</param>
        /// <param name="pluginLoader">Service for loading game plugins</param>
        public GameService(
            ILogger<GameService> logger, 
            IServiceProvider serviceProvider,
            AnimalConfigurationService animalConfig,
            PluginLoader pluginLoader)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _animalConfig = animalConfig;
            _pluginLoader = pluginLoader;
            
            _logger.LogInformation(GameServiceConstants.LogMessages.ServiceInitialized, _games.Count);
        }

        /// <summary>
        /// Gets or creates a game ID for a user, ensuring thread safety.
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>The game ID associated with the user</returns>
        private string GetOrCreateGameId(string userId)
        {
            _logger.LogInformation(GameServiceConstants.LogMessages.GettingGameId, userId);
            
            return _userGameMap.GetOrAdd(userId, _ =>
            {
                lock (_lock)
                {
                    if (_userGameMap.TryGetValue(userId, out var existingId))
                    {
                        _logger.LogInformation(GameServiceConstants.LogMessages.ExistingGameFound, existingId, userId);
                        return existingId;
                    }

                    var newId = Guid.NewGuid().ToString();
                    var newGame = new GameInstance(_logger);
                    newGame.InitializeGame();

                    if (!_games.TryAdd(newId, newGame))
                    {
                        _logger.LogError(GameServiceConstants.LogMessages.FailedToAddGame, newId);
                        throw new GameServiceException(ExceptionMessages.Game.GameAlreadyExists, userId, newId);
                    }

                    _logger.LogInformation(GameServiceConstants.LogMessages.NewGameCreated, newId, userId);
                    return newId;
                }
            });
        }

        /// <summary>
        /// Gets the game instance associated with a user.
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>The game instance if found, null otherwise</returns>
        private GameInstance? GetUserGame(string userId)
        {
            var gameId = _userGameMap.GetValueOrDefault(userId);
            if (string.IsNullOrEmpty(gameId))
            {
                _logger.LogWarning(GameServiceConstants.LogMessages.NoGameIdFound, userId);
                return null;
            }

            if (_games.TryGetValue(gameId, out var game))
            {
                return game;
            }

            _logger.LogWarning(GameServiceConstants.LogMessages.GameInstanceNotFound, gameId);
            return null;
        }

        /// <summary>
        /// Starts a new game for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public Task StartGame(string userId)
        {
            try
            {
                _logger.LogInformation(GameServiceConstants.LogMessages.StartingNewGame, userId);
                
                // First, ensure any existing game is properly cleaned up
                var existingGameId = _userGameMap.GetValueOrDefault(userId);
                if (!string.IsNullOrEmpty(existingGameId))
                {
                    if (_games.TryRemove(existingGameId, out var oldGame))
                    {
                        oldGame?.Quit();
                    }
                    _userGameMap.TryRemove(userId, out _);
                }

                // Get a new game ID - this will create and initialize the game instance
                var gameId = GetOrCreateGameId(userId);
                
                _logger.LogInformation(GameServiceConstants.LogMessages.GameStarted, userId, gameId);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GameServiceConstants.LogMessages.FailedToStartGame, userId);
                // Clean up any partial initialization
                if (_userGameMap.TryGetValue(userId, out var failedGameId))
                {
                    _games.TryRemove(failedGameId, out _);
                    _userGameMap.TryRemove(userId, out _);
                }
                throw new GameServiceException(ExceptionMessages.Game.GameNotFound, userId, ex);
            }
        }

        /// <summary>
        /// Quits the game for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public Task QuitGame(string userId)
        {
            var gameId = _userGameMap.GetValueOrDefault(userId);
            if (!string.IsNullOrEmpty(gameId) && _games.TryRemove(gameId, out var game))
            {
                game.Quit();
                _userGameMap.TryRemove(userId, out _);
                _logger.LogInformation(GameServiceConstants.LogMessages.GameQuit, userId);
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Adds an animal of the specified type to the user's game.
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <param name="type">The type of animal to add</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public async Task AddAnimal(string userId, string type)
        {
            try
            {
                _logger.LogInformation(GameServiceConstants.LogMessages.AddingAnimal, type, userId);

                var gameId = _userGameMap.GetValueOrDefault(userId);
                if (string.IsNullOrEmpty(gameId))
                {
                    _logger.LogWarning(GameServiceConstants.LogMessages.NoActiveGame);
                    throw new GameServiceException(ExceptionMessages.Game.GameNotFound, userId);
                }

                if (!_games.TryGetValue(gameId, out var game))
                {
                    _logger.LogWarning(GameServiceConstants.LogMessages.GameInstanceNotFound);
                    throw new GameServiceException(ExceptionMessages.Game.GameNotFound, userId, gameId);
                }

                // Get the animal configuration to validate the type
                var configs = _animalConfig.GetAnimalConfigurations();
                var config = configs.FirstOrDefault(c => 
                    c.Value.Plugin.Name.Equals(type, StringComparison.OrdinalIgnoreCase)).Value;
                
                if (config == null)
                {
                    _logger.LogWarning(GameServiceConstants.LogMessages.ConfigNotFound, type);
                    throw new GameServiceException($"Invalid animal type: {type}", userId, gameId);
                }

                var symbol = config.Configuration.Symbol;
                _logger.LogInformation(GameServiceConstants.LogMessages.UsingSymbol, symbol, type);

                // Add the animal using the symbol from configuration
                game.AddAnimal(symbol[0], new Common.Models.Position(GameServiceConstants.GameDefaults.InvalidPosition, GameServiceConstants.GameDefaults.InvalidPosition));

                _logger.LogInformation(GameServiceConstants.LogMessages.AnimalAdded, type, symbol, userId);
                
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GameServiceConstants.LogMessages.FailedToAddAnimal, type, userId);
                throw;
            }
        }

        /// <summary>
        /// Gets the current game state for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>The current game state, or null if no game exists</returns>
        public GameStateDto? GetGameState(string userId)
        {
            _logger.LogInformation(GameServiceConstants.LogMessages.GettingGameState, userId, _games.Count);
            _logger.LogInformation(GameServiceConstants.LogMessages.UserMappings, 
                string.Join(", ", _userGameMap.Select(kvp => $"{kvp.Key}={kvp.Value}")));
            
            var gameId = _userGameMap.GetValueOrDefault(userId);
            if (string.IsNullOrEmpty(gameId))
            {
                _logger.LogWarning(GameServiceConstants.LogMessages.NoGameIdInMapping, userId);
                return null;
            }

            if (!_games.TryGetValue(gameId, out var game))
            {
                _logger.LogWarning(GameServiceConstants.LogMessages.GameNotFoundInDict, gameId, userId);
                return null;
            }

            var state = game.GetGameState();
            _logger.LogInformation(GameServiceConstants.LogMessages.GameStateRetrieved, 
                userId, state.Iteration, state.Animals.Count);
            
            return state;
        }

        /// <summary>
        /// Saves the current game state for the specified user.
        /// </summary>
        /// <param name="userEmail">The email of the user</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public async Task SaveGame(string userEmail)
        {
            try
            {
                _logger.LogInformation(GameServiceConstants.LogMessages.SavingGame, userEmail);

                var game = GetUserGame(userEmail);
                if (game == null)
                {
                    _logger.LogWarning(GameServiceConstants.LogMessages.NoActiveGame);
                    throw new GameServiceException(ExceptionMessages.Game.GameNotFound, userEmail);
                }

                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserName == userEmail);
                if (user == null)
                {
                    _logger.LogWarning(GameServiceConstants.LogMessages.UserNotFound, userEmail);
                    throw new GameServiceException(ExceptionMessages.User.UserNotFound, userEmail);
                }

                var gameState = game.GetGameState();
                if (gameState == null)
                {
                    _logger.LogError(GameServiceConstants.ErrorMessages.FailedToGetGameState);
                    throw new GameServiceException(ExceptionMessages.Game.InvalidGameState, userEmail);
                }

                _logger.LogInformation(GameServiceConstants.LogMessages.CreatingGameState, 
                    gameState.Animals.Count, user.Id);

                var gameStateEntity = new Infrastructure.Models.GameState
                {
                    BoardWidth = gameState.Width,
                    BoardHeight = gameState.Height,
                    CurrentIteration = gameState.Iteration,
                    Animals = gameState.Animals.Select(a => new Infrastructure.Models.AnimalState
                    {
                        AnimalType = a.Type,
                        PositionX = a.X,
                        PositionY = a.Y,
                        Health = a.Health,
                        IsAlive = a.IsAlive
                    }).ToList()
                };

                var gameSave = new Infrastructure.Models.GameSave
                {
                    Name = string.Format(GameServiceConstants.GameDefaults.GameSaveNameFormat, DateTime.Now),
                    SaveDate = DateTime.Now,
                    UserId = user.Id,
                    GameState = gameStateEntity
                };

                try
                {
                    _logger.LogInformation(GameServiceConstants.LogMessages.AddingToDatabase, user.Id);
                    await dbContext.GameStates.AddAsync(gameStateEntity);
                    await dbContext.GameSaves.AddAsync(gameSave);
                    
                    var changes = await dbContext.SaveChangesAsync();
                    _logger.LogInformation(GameServiceConstants.LogMessages.GameSaved, 
                        changes, userEmail, user.Id);
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, GameServiceConstants.LogMessages.DatabaseError, ex.InnerException?.Message ?? ex.Message);
                    throw new GameServiceException("Failed to save game to database", userEmail, ex);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, GameServiceConstants.LogMessages.ErrorSavingGame, userEmail, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Loads a saved game state for the specified user.
        /// </summary>
        /// <param name="saveId">The ID of the saved game to load</param>
        /// <param name="userId">The ID of the user</param>
        /// <returns>A task representing the asynchronous operation</returns>
        public async Task LoadGame(int saveId, string userId)
        {
            _logger.LogInformation(GameServiceConstants.LogMessages.LoadingGame, userId, saveId);

            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserName == userId);
            if (user == null)
            {
                _logger.LogWarning(GameServiceConstants.LogMessages.UserNotFound, userId);
                throw new GameServiceException(ExceptionMessages.User.UserNotFound, userId);
            }

            var save = await dbContext.GameSaves
                .Include(g => g.GameState)
                .ThenInclude(s => s.Animals)
                .FirstOrDefaultAsync(g => g.Id == saveId && g.UserId == user.Id);

            if (save == null || save.GameState == null)
            {
                _logger.LogWarning(GameServiceConstants.LogMessages.GameInstanceNotFound, saveId);
                throw new GameServiceException(ExceptionMessages.Game.SaveNotFound, userId);
            }

            // First, ensure any existing game is cleaned up
            if (_userGameMap.TryGetValue(userId, out var existingGameId))
            {
                if (_games.TryRemove(existingGameId, out var oldGame))
                {
                    oldGame?.Quit();
                }
                _userGameMap.TryRemove(userId, out _);
            }

            var game = new GameInstance(_logger);
            game.InitializeGame();

            // Set the iteration from the saved game state
            game.SetIteration(save.GameState.CurrentIteration);

            // Load all animals from the save
            foreach (var animal in save.GameState.Animals.Where(a => a.IsAlive))
            {
                _logger.LogInformation(GameServiceConstants.LogMessages.LoadingAnimal, 
                    animal.AnimalType, animal.PositionX, animal.PositionY);
                    
                game.AddAnimal(animal.AnimalType[0], 
                    new Common.Models.Position(animal.PositionX, animal.PositionY));
            }

            // Add the game to our dictionaries
            var newGameId = Guid.NewGuid().ToString();
            if (_games.TryAdd(newGameId, game))
            {
                _userGameMap[userId] = newGameId;
                _logger.LogInformation(GameServiceConstants.LogMessages.GameStarted, userId, saveId);
            }
            else
            {
                _logger.LogError(GameServiceConstants.LogMessages.FailedToInitialize);
                throw new GameServiceException(ExceptionMessages.Game.GameAlreadyExists, userId, newGameId);
            }
        }

        /// <summary>
        /// Gets detailed information about a specific animal.
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <param name="animalId">The ID of the animal</param>
        /// <returns>Detailed information about the animal, or null if not found</returns>
        public AnimalDetailsDto? GetAnimalDetails(string userId, string animalId)
        {
            var game = GetUserGame(userId);
            if (game == null)
            {
                _logger.LogWarning(GameServiceConstants.LogMessages.NoActiveGame);
                return null;
            }

            var animal = game.GetAnimal(animalId);
            if (animal == null)
            {
                _logger.LogWarning(GameServiceConstants.LogMessages.AnimalNotFound, animalId);
                return null;
            }

            var state = game.GetAnimalState(animal);
            if (state == null)
            {
                _logger.LogWarning(GameServiceConstants.LogMessages.AnimalStateNotFound, animalId);
                return null;
            }

            return new AnimalDetailsDto
            {
                Id = animalId,
                Type = animal.GetType().Name,
                X = animal.Position.X,
                Y = animal.Position.Y,
                Health = (int)animal.Health,
                IsAlive = animal.IsAlive,
                IsSelected = game.IsAnimalSelected(animalId),
                Age = state.Age,
                OffspringCount = state.OffspringCount
            };
        }

        /// <summary>
        /// Toggles the pause state of the game for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>The new pause state of the game</returns>
        public bool TogglePause(string userId)
        {
            var game = GetUserGame(userId);
            if (game == null)
            {
                _logger.LogWarning(GameServiceConstants.LogMessages.AttemptToPause, userId);
                return false;
            }

            game.IsPaused = !game.IsPaused;
            _logger.LogInformation(GameServiceConstants.LogMessages.GamePauseState, 
                game.IsPaused ? "paused" : "resumed", userId);
            return game.IsPaused;
        }

        /// <summary>
        /// Updates the game state for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>The updated game state, or null if no game exists</returns>
        public async Task<GameStateDto?> UpdateGameStateAsync(string userId)
        {
            _logger.LogInformation(GameServiceConstants.LogMessages.UpdatingGameState, userId);

            var game = GetUserGame(userId);
            if (game == null)
            {
                _logger.LogWarning(GameServiceConstants.LogMessages.AttemptToUpdateNonExistent, userId);
                return null;
            }

            if (!game.IsPaused)
            {
                game.Update();
            }
            return await Task.FromResult(game.GetGameState());
        }

        /// <summary>
        /// Gets a list of all available animal types that can be added to the game.
        /// </summary>
        /// <returns>A collection of animal type names</returns>
        public IEnumerable<string> GetAvailableAnimalTypes()
        {
            return _animalConfig.GetAnimalConfigurations()
                .Select(c => c.Value.Plugin.Name)
                .OrderBy(name => name);
        }

        // ... (rest of the GameService methods) ...
    }
} 