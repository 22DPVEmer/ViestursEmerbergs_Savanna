using Savanna.Common.Models;

namespace Savanna.Common.Interfaces
{
    /// <summary>
    /// Defines reproduction-related behaviors for game entities
    /// </summary>
    public interface IReproducible
    {
        int ConsecutiveRoundsNearMate { get; }
        bool CanReproduce { get; }
        void UpdateReproductionStatus(IGameField field);
        IGameEntity Reproduce(Position position);
    }
} 