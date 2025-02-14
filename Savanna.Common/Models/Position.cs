using System;

namespace Savanna.Common.Models
{
    /// <summary>
    /// Represents a position on the game field
    /// </summary>
    public record Position(int X, int Y)
    {
        /// <summary>
        /// Calculates the distance to another position
        /// </summary>
        public double DistanceTo(Position other)
        {
            int dx = X - other.X;
            int dy = Y - other.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
} 