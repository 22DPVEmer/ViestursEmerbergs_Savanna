namespace Savanna.Common.Interfaces
{
    /// <summary>
    /// Defines configuration properties for animals.
    /// This interface bridges game constants with entity properties.
    /// </summary>
    public interface IAnimalConfiguration
    {
        /// <summary>
        /// Movement speed of the animal
        /// </summary>
        int Speed { get; }

        /// <summary>
        /// How far the animal can see
        /// </summary>
        int VisionRange { get; }

        /// <summary>
        /// Character representation on the game field
        /// </summary>
        char Symbol { get; }
    }
} 