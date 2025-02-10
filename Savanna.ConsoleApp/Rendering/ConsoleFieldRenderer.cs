using Savanna.GameEngine.Constants;
using Savanna.GameEngine.Models;
using System.Collections.Generic;
using System.Linq;

namespace Savanna.ConsoleApp.Rendering
{
    /// <summary>
    /// Default console-based field renderer.
    /// Handles the visual representation of the game field.
    /// </summary>
    public class ConsoleFieldRenderer : IFieldRenderer
    {
        public char[,] RenderField(int width, int height, IReadOnlyList<Animal> animals)
        {
            var field = InitializeEmptyField(width, height);
            PlaceAnimalsOnField(field, animals);
            return field;
        }

        private char[,] InitializeEmptyField(int width, int height)
        {
            var field = new char[height, width];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    field[y, x] = GameConstants.Field.EmptyCell;
            return field;
        }

        private void PlaceAnimalsOnField(char[,] field, IReadOnlyList<Animal> animals)
        {
            int height = field.GetLength(0);
            int width = field.GetLength(1);

            foreach (var animal in animals.Where(a => a.IsAlive))
            {
                if (IsValidPosition(animal.Position, width, height))
                {
                    field[animal.Position.Y, animal.Position.X] = animal.Symbol;
                }
            }
        }

        private bool IsValidPosition(Position position, int width, int height)
        {
            return position.X >= 0 && position.X < width &&
                   position.Y >= 0 && position.Y < height;
        }
    }
} 