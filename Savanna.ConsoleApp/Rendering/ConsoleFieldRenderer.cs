using System.Collections.Generic;
using Savanna.GameEngine.Constants;
using Savanna.Common.Interfaces;

namespace Savanna.ConsoleApp.Rendering
{
    /// <summary>
    /// Renders the game field for console display
    /// </summary>
    public class ConsoleFieldRenderer : IFieldRenderer
    {
        /// <summary>
        /// Creates a 2D array representing the field state
        /// </summary>
        public char[,] RenderField(int width, int height, IReadOnlyList<IGameEntity> animals)
        {
            var field = new char[height, width];

            // Fill field with empty cells
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    field[y, x] = GameConstants.Field.EmptyCell;
                }
            }

            // Place animals on the field
            foreach (var animal in animals)
            {
                if (animal.IsAlive)
                {
                    field[animal.Position.Y, animal.Position.X] = animal.Symbol;
                }
            }

            return field;
        }
    }
} 