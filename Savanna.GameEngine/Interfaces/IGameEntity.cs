using Savanna.GameEngine.Models;

namespace Savanna.GameEngine.Interfaces
{
    /// <summary>
    /// Defines common properties for game entities
    /// </summary>
    public interface IGameEntity : IMovable, IActionable
    {
        Position Position { get; set; }
        int Speed { get; }
        int VisionRange { get; }
        char Symbol { get; }
        bool IsAlive { get; set; }
    }
} 