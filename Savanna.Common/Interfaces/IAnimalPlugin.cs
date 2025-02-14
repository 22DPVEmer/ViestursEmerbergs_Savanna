using Savanna.Common.Models;

namespace Savanna.Common.Interfaces
{
    /// <summary>
    /// Main contract for animal plugins. All animal plugins must implement this interface.
    /// </summary>
    public interface IAnimalPlugin
    {
        /// <summary>
        /// Unique identifier for the animal type
        /// </summary>
        string AnimalName { get; }
        
        /// <summary>
        /// The symbol that represents the animal on the game field
        /// </summary>
        char Symbol { get; }
        
        /// <summary>
        /// Animal configuration containing basic properties
        /// </summary>
        IAnimalConfiguration Configuration { get; }
        
        /// <summary>
        /// Factory method to create a new instance of the animal
        /// </summary>
        IGameEntity CreateAnimal(Position position);

        /// <summary>
        /// Version of the plugin
        /// </summary>
        string Version => "1.0.0";

        /// <summary>
        /// Validates if the plugin is compatible with the current game version
        /// </summary>
        bool IsCompatible => true;
    }
} 