namespace Savanna.GameEngine.Interfaces
{
    /// <summary>
    /// Defines configuration properties for animals.
    /// This interface bridges game constants with entity properties.
    /// </summary>
    public interface IAnimalConfiguration
    {
        int Speed { get; }
        int VisionRange { get; }
        char Symbol { get; }
    }
} 