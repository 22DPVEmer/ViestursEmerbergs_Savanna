using Savanna.Common.Models;
using System.Collections.Generic;

namespace Savanna.Common.Interfaces
{
    /// <summary>
    /// Defines the interface for the game field that animals interact with
    /// </summary>
    public interface IGameField
    {
        /// <summary>
        /// Width of the game field
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Height of the game field
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Gets all entities at a specific position
        /// </summary>
        IEnumerable<IGameEntity> GetEntitiesAt(Position position);

        /// <summary>
        /// Gets all entities within a certain range of a position
        /// </summary>
        IEnumerable<IGameEntity> GetEntitiesInRange(Position position, int range);

        /// <summary>
        /// Checks if a position is within the field boundaries
        /// </summary>
        bool IsValidPosition(Position position);

        /// <summary>
        /// Gets all entities of a specific type within range
        /// </summary>
        IEnumerable<T> GetEntitiesOfTypeInRange<T>(Position position, int range) where T : IGameEntity;

        /// <summary>
        /// Gets all animals in the game field
        /// </summary>
        IReadOnlyList<IGameEntity> Animals { get; }

        /// <summary>
        /// Clamps a position to be within the field boundaries
        /// </summary>
        Position ClampPosition(Position position);

        /// <summary>
        /// Adds an animal to the game field
        /// </summary>
        void AddAnimal(char type, Position position);

        /// <summary>
        /// Updates the game field
        /// </summary>
        void Update();

        /// <summary>
        /// Checks if the field has reached its maximum capacity
        /// </summary>
        bool IsAtCapacity();
    }
} 