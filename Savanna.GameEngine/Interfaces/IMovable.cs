namespace Savanna.GameEngine.Interfaces
{
    /// <summary>
    /// Defines movement behavior for game entities
    /// </summary>
    public interface IMovable
    {
        /// <summary>
        /// Calculates and performs movement within the game field
        /// </summary>
        void Move(GameField field);
    }
} 