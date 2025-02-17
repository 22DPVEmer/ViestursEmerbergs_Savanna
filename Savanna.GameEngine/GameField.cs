using Savanna.GameEngine.Constants;
using Savanna.GameEngine.Models;
using Savanna.Common.Models;
using Savanna.Common.Interfaces;

namespace Savanna.GameEngine
{
    /// <summary>
    /// Represents the game field where animals move and interact.
    /// Manages the collection of animals and their state updates.
    /// Acts as the main coordinator for the game's logic.
    /// </summary>
    public class GameField : IGameField
    {
        /// <summary>
        /// Private list of animals for internal management.
        /// Using List for efficient add/remove operations.
        /// </summary>
        private readonly List<IGameEntity> _animals;
        private readonly IAnimalFactory _animalFactory;

        /// <summary>
        /// Width of the game field in cells.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Height of the game field in cells.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Public read-only access to animals list.
        /// Returns AsReadOnly to prevent external modifications.
        /// </summary>
        public IReadOnlyList<IGameEntity> Animals => _animals.AsReadOnly();

        /// <summary>
        /// Creates a new game field with specified or default dimensions.
        /// Uses constants for consistent game setup.
        /// </summary>
        public GameField(
            IAnimalFactory animalFactory,
            int width = GameConstants.Field.DefaultWidth,
            int height = GameConstants.Field.DefaultHeight)
        {
            Width = width;
            Height = height;
            _animals = new List<IGameEntity>();
            _animalFactory = animalFactory ?? throw new ArgumentNullException(nameof(animalFactory));
        }

        /// <summary>
        /// Gets all entities at a specific position
        /// </summary>
        public IEnumerable<IGameEntity> GetEntitiesAt(Position position)
        {
            return _animals.Where(a => a.IsAlive && a.Position.Equals(position));
        }

        /// <summary>
        /// Gets all entities within a certain range of a position
        /// </summary>
        public IEnumerable<IGameEntity> GetEntitiesInRange(Position position, int range)
        {
            return _animals.Where(a => a.IsAlive && a.Position.DistanceTo(position) <= range);
        }

        /// <summary>
        /// Gets all entities of a specific type within range
        /// </summary>
        public IEnumerable<T> GetEntitiesOfTypeInRange<T>(Position position, int range) where T : IGameEntity
        {
            return GetEntitiesInRange(position, range).OfType<T>();
        }

        /// <summary>
        /// Validates if a position is within the field bounds
        /// </summary>
        public bool IsValidPosition(Position position)
        {
            return position.X >= 0 && position.X < Width &&
                   position.Y >= 0 && position.Y < Height;
        }

        /// <summary>
        /// Ensures a position stays within field bounds
        /// </summary>
        public Position ClampPosition(Position position)
        {
            return new Position(
                Math.Clamp(position.X, 0, Width - 1),
                Math.Clamp(position.Y, 0, Height - 1)
            );
        }

        /// <summary>
        /// Checks if the field has reached its maximum capacity
        /// </summary>
        public bool IsAtCapacity()
        {
            return _animals.Count(a => a.IsAlive) >= Width * Height;
        }

        /// <summary>
        /// Adds a new animal to the field based on type.
        /// Factory method pattern for creating animals.
        /// </summary>
        public void AddAnimal(char type, Position position)
        {
            // Clean up dead animals before checking capacity
            _animals.RemoveAll(a => !a.IsAlive);
            
            // Don't add more animals if we're at capacity
            if (IsAtCapacity())
            {
                return;
            }

            position = ClampPosition(position);
            var animal = _animalFactory.CreateAnimal(type, position);
            _animals.Add(animal);
        }

        /// <summary>
        /// Main update method called each game tick.
        /// Updates positions and actions for all active entities.
        /// </summary>
        public void Update()
        {
            // Clean up dead animals before processing updates
            _animals.RemoveAll(a => !a.IsAlive);
            
            var activeEntities = GetActiveEntities();
            UpdateEntities(activeEntities);
        }

        /// <summary>
        /// Gets list of all living entities.
        /// Uses LINQ for efficient filtering.
        /// Ensures count cannot exceed field capacity.
        /// </summary>
        private List<IGameEntity> GetActiveEntities()
        {
            var maxEntities = Width * Height;
            var activeEntities = _animals.Where(a => a.IsAlive).ToList();
            
            // If we somehow got more entities than spaces, prioritize keeping the healthiest entities
            if (activeEntities.Count > maxEntities)
            {
                activeEntities = activeEntities
                    .OrderByDescending(a => (a as IHealthManageable)?.Health ?? 0)
                    .Take(maxEntities)
                    .ToList();
                
                // Remove any entities that didn't make the cut
                _animals.RemoveAll(a => !activeEntities.Contains(a));
            }
            
            return activeEntities;
        }

        /// <summary>
        /// Updates all active entities in sequence.
        /// Each entity:
        /// 1. Moves based on its behavior (if IMovable)
        /// 2. Performs any special actions (if IActionable)
        /// </summary>
        private void UpdateEntities(List<IGameEntity> activeEntities)
        {
            // Only try to use console if we're not in a test environment
            try
            {
                Console.SetCursorPosition(0, Height + 5);
                Console.WriteLine(string.Format(GameFieldConstants.Messages.UpdatingEntities, activeEntities.Count));
            }
            catch (IOException)
            {
                // Ignore console errors in test environment
            }
            
            foreach (var entity in activeEntities)
            {
                if (entity is IMovable movable)
                {
                    movable.Move(this);
                }
                
                if (entity is IActionable actionable)
                {
                    actionable.PerformAction(this);
                }
            }
        }
    }
}