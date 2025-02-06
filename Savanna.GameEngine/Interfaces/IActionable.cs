namespace Savanna.GameEngine.Interfaces
{
    /// <summary>
    /// Defines special action behavior for game entities
    /// </summary>
    public interface IActionable
    {
        /// <summary>
        /// Performs special actions within the game field
        /// </summary>
        void PerformAction(GameField field);
    }
} 