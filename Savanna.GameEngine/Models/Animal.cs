using System;
using Savanna.GameEngine.Interfaces;

namespace Savanna.GameEngine.Models
{
    /// <summary>
    /// Base class for all animals in the game.
    /// Provides common properties and behaviors that all animals share.
    /// Uses the Template Method pattern for movement and actions.
    /// </summary>
    public abstract class Animal : IGameEntity
    {
        private readonly IAnimalConfiguration _configuration;

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
        /// Indicates if the animal is alive. Public set allows for interaction between animals
        /// (e.g., Lion catching Antelope).
        /// </summary>
        public bool IsAlive { get; set; } = true;

        /// <summary>
        /// Base constructor that initializes common animal properties.
        /// Protected to ensure it can only be called by derived classes.
        /// </summary>
        protected Animal(Position position, IAnimalConfiguration configuration)
        {
            Position = position;
            _configuration = configuration;
        }

        /// <summary>
        /// Abstract method for movement behavior.
        /// Each animal type must implement its own movement logic.
        /// </summary>
        public abstract void Move(GameField field);

        /// <summary>
        /// Abstract method for special actions.
        /// Each animal type must implement its own action logic
        /// (e.g., Lions catching Antelopes).
        /// </summary>
        public abstract void PerformAction(GameField field);
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