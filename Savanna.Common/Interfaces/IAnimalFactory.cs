using Savanna.Common.Models;

namespace Savanna.Common.Interfaces
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

        /// <summary>
        /// Gets all available animal types, including both built-in and plugin-based animals
        /// </summary>
        IEnumerable<char> GetAvailableAnimalTypes();
    }
} 