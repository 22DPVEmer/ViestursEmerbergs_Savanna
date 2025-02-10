using System;
using Savanna.GameEngine.Interfaces;
using Savanna.GameEngine.Constants;

namespace Savanna.GameEngine.Models
{
    /// <summary>
    /// Base class for all animals in the game.
    /// Provides common properties and behaviors that all animals share.
    /// Uses the Template Method pattern for movement and actions.
    /// </summary>
    public abstract class Animal : IGameEntity, IHealthManageable, IReproducible
    {
        private readonly IAnimalConfiguration _configuration;
        private readonly HealthManager _healthManager;
        private readonly ReproductionManager _reproductionManager;

        /// <summary>
        /// Current position on the field. Public set allows for movement updates.
        /// </summary>
        public Position Position { get; set; }

        /// <summary>
        /// Movement speed from configuration
        /// </summary>
        public int Speed => _configuration.Speed;

        /// <summary>
        /// Vision range from configuration
        /// </summary>
        public int VisionRange => _configuration.VisionRange;

        /// <summary>
        /// Symbol from configuration
        /// </summary>
        public char Symbol => _configuration.Symbol;

        /// <summary>
        /// Health-related properties delegated to HealthManager
        /// </summary>
        public double Health => _healthManager.Health;
        public bool IsAlive => _healthManager.IsAlive;

        /// <summary>
        /// Reproduction-related properties delegated to ReproductionManager
        /// </summary>
        public int ConsecutiveRoundsNearMate => _reproductionManager.ConsecutiveRoundsNearMate;
        public bool CanReproduce => _reproductionManager.CanReproduce;

        /// <summary>
        /// Base constructor that initializes common animal properties.
        /// Protected to ensure it can only be called by derived classes.
        /// </summary>
        protected Animal(Position position, IAnimalConfiguration configuration)
        {
            Position = position;
            _configuration = configuration;
            _healthManager = new HealthManager();
            _reproductionManager = CreateReproductionManager();
        }

        /// <summary>
        /// Internal method to access configuration from derived classes and managers
        /// </summary>
        internal IAnimalConfiguration GetConfiguration() => _configuration;

        /// <summary>
        /// Factory method for creating the appropriate reproduction manager
        /// </summary>
        protected abstract ReproductionManager CreateReproductionManager();

        /// <summary>
        /// Abstract method for movement behavior.
        /// Each animal type must implement its own movement logic.
        /// </summary>
        public virtual void Move(GameField field)
        {
            if (!IsAlive) return;

            PerformMove(field);
            _healthManager.DecreaseHealth(GameConstants.Health.MovementHealthCost);
        }

        /// <summary>
        /// Template method for actual movement implementation
        /// </summary>
        protected abstract void PerformMove(GameField field);

        /// <summary>
        /// Abstract method for special actions.
        /// Each animal type must implement its own action logic
        /// (e.g., Lions catching Antelopes).
        /// </summary>
        public virtual void PerformAction(GameField field)
        {
            if (!IsAlive) return;

            _reproductionManager.UpdateReproductionStatus(field);
            
            if (_reproductionManager.CanReproduce)
            {
                var offspring = _reproductionManager.Reproduce(Position);
                field.AddAnimal(offspring.Symbol, offspring.Position);
                DecreaseHealth(GameConstants.Reproduction.ReproductionHealthCost);
            }

            // Skip special action for Antelopes since they don't have any
            if (!(this is Antelope))
            {
                PerformSpecialAction(field);
            }
        }

        /// <summary>
        /// Template method for special action implementation
        /// </summary>
        protected abstract void PerformSpecialAction(GameField field);

        // Health management methods delegated to HealthManager
        public void DecreaseHealth(double amount) => _healthManager.DecreaseHealth(amount);
        public void IncreaseHealth(double amount) => _healthManager.IncreaseHealth(amount);
        public void Die() => _healthManager.Die();

        // Reproduction methods delegated to ReproductionManager
        public void UpdateReproductionStatus(GameField field) => _reproductionManager.UpdateReproductionStatus(field);
        public abstract IGameEntity Reproduce(Position position);
    }

    /// <summary>
    /// Immutable record representing a position on the field.
    /// Uses record for built-in value equality and immutability.
    /// </summary>
    public record Position(int X, int Y)
    {
        /// <summary>
        /// Calculates Euclidean distance between two positions.
        /// Used for determining distances between animals for vision and interaction.
        /// </summary>
        public double DistanceTo(Position other)
        {
            int dx = X - other.X;
            int dy = Y - other.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
} 