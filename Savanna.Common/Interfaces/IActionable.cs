using Savanna.Common.Interfaces;

namespace Savanna.Common.Interfaces
{
    /// <summary>
    /// Defines special action behavior for game entities
    /// </summary>
    public interface IActionable
    {
        /// <summary>
        /// Performs special actions within the game field
        /// </summary>
        void PerformAction(IGameField field);
    }
} 