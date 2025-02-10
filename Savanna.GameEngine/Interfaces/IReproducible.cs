using Savanna.GameEngine.Models;

namespace Savanna.GameEngine.Interfaces
{
    /// <summary>
    /// Defines reproduction-related behaviors for game entities
    /// </summary>
    public interface IReproducible
    {
        int ConsecutiveRoundsNearMate { get; }
        bool CanReproduce { get; }
        void UpdateReproductionStatus(GameField field);
        IGameEntity Reproduce(Position position);
    }
} 