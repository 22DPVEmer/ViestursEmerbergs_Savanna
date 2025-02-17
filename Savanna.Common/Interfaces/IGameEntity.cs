using Savanna.Common.Models;

namespace Savanna.Common.Interfaces
{
    /// <summary>
    /// Defines the basic contract for any entity in the game
    /// </summary>
    public interface IGameEntity
    {
        /// <summary>
        /// Current position of the entity on the game field
        /// </summary>
        Position Position { get; set; }

        /// <summary>
        /// Character representation of the entity
        /// </summary>
        char Symbol { get; }

        /// <summary>
        /// Whether the entity is still alive
        /// </summary>
        bool IsAlive { get; }

        /// <summary>
        /// Current health value of the entity
        /// </summary>
        double Health { get; }
    }
} 