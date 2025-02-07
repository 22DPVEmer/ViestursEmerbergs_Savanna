using Savanna.GameEngine.Models;

namespace Savanna.GameEngine.Interfaces
{
    /// <summary>
    /// Factory interface for creating game entities.
    /// Abstracts animal creation from game logic.
    /// </summary>
    public interface IAnimalFactory
    {
        /// <summary>
        /// Creates an animal of specified type at given position
        /// </summary>
        IGameEntity CreateAnimal(char type, Position position);
    }
} 