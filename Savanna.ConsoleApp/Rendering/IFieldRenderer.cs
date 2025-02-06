using Savanna.GameEngine.Models;
using System.Collections.Generic;

namespace Savanna.ConsoleApp.Rendering
{
    /// <summary>
    /// Interface for rendering the game field.
    /// Separates display logic from game logic.
    /// </summary>
    public interface IFieldRenderer
    {
        /// <summary>
        /// Creates a visual representation of the field state
        /// </summary>
        char[,] RenderField(int width, int height, IReadOnlyList<Animal> animals);
    }
} 