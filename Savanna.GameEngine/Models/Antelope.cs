using Savanna.GameEngine.Constants;
using Savanna.GameEngine.Interfaces;
using System;
using System.Linq;

namespace Savanna.GameEngine.Models
{
    /// <summary>
    /// Represents an Antelope in the game.
    /// Antelopes are prey animals that try to survive by avoiding Lions.
    /// They move faster than Lions but have shorter vision range.
    /// </summary>
    public class Antelope : Animal
    {
        // Add shared Random instance like in Lion class to prevent same-tick values
        private static readonly Random Random = new Random();

        /// <summary>
        /// Creates a new Antelope at the specified position.
        /// Uses constants for consistent game balance.
        /// </summary>
        public Antelope(Position position, IAnimalConfiguration configuration) 
            : base(position, configuration)
        {
        }

        protected override ReproductionManager CreateReproductionManager()
        {
            return new AntelopeReproductionManager(this, this);
        }

        public override IGameEntity Reproduce(Position position)
        {
            return new Antelope(position, GetConfiguration());
        }

        protected override void PerformMove(GameField field)
        {
            var nearestLion = FindNearestLion(field);
            
            Position = nearestLion != null && IsLionInRange(nearestLion)
                ? CalculateEscapePosition(field, nearestLion)
                : CalculateRandomPosition(field);
        }

        protected override void PerformSpecialAction(GameField field)
        {
            // Antelopes don't have special actions yet
            // Could be extended for features like grazing
        }

        /// <summary>
        /// Finds the nearest living Lion on the field.
        /// Uses LINQ for efficient filtering and ordering.
        /// Returns null if no Lions are alive.
        /// </summary>
        private Lion? FindNearestLion(GameField field) =>
            field.Animals
                .OfType<Lion>()
                .Where(l => l.IsAlive)
                .OrderBy(l => l.Position.DistanceTo(Position))
                .FirstOrDefault();

        /// <summary>
        /// Checks if a Lion is within the Antelope's vision range.
        /// This determines whether the Antelope needs to flee.
        /// </summary>
        private bool IsLionInRange(Lion lion) =>
            Position.DistanceTo(lion.Position) <= VisionRange;

        /// <summary>
        /// Calculates the best escape position when a Lion is nearby.
        /// Uses vector math to move directly away from the Lion:
        /// 1. Calculate direction vector from Lion to Antelope
        /// 2. Normalize direction using Math.Sign
        /// 3. Multiply by speed to get movement distance
        /// 4. Ensure new position is within field bounds
        /// </summary>
        private Position CalculateEscapePosition(GameField field, Lion lion)
        {
            // Calculate base direction away from lion
            int dx = Position.X - lion.Position.X;
            int dy = Position.Y - lion.Position.Y;

            // If perfectly aligned, add slight randomness to break alignment
            if (dx == 0) dx = Random.Next(GameConstants.Movement.MinDirectionOffset, GameConstants.Movement.MaxDirectionOffset + 1);
            if (dy == 0) dy = Random.Next(GameConstants.Movement.MinDirectionOffset, GameConstants.Movement.MaxDirectionOffset + 1);

            // Try multiple escape directions in priority order
            var possibleMoves = new[]
            {
                TryMove(field, dx, dy),                    // Direct escape
                TryMove(field, dx, -dy),                   // Perpendicular 1
                TryMove(field, -dx, dy),                   // Perpendicular 2
                TryMove(field, -dx, -dy),                  // Opposite direction
                TryMove(field, dx * 2, 0),                 // Horizontal escape
                TryMove(field, 0, dy * 2),                 // Vertical escape
                TryMove(field,                             // Random escape as last resort
                    Random.Next(GameConstants.Movement.MinDirectionOffset, GameConstants.Movement.MaxDirectionOffset + 1) * Speed,
                    Random.Next(GameConstants.Movement.MinDirectionOffset, GameConstants.Movement.MaxDirectionOffset + 1) * Speed)
            };

            // Return the first valid move that increases distance from lion
            var currentDistance = Position.DistanceTo(lion.Position);
            foreach (var move in possibleMoves)
            {
                if (move.DistanceTo(lion.Position) > currentDistance)
                    return move;
            }

            // If no better position found, at least try to move somewhere
            return possibleMoves.First();
        }

        private Position TryMove(GameField field, int dx, int dy)
        {
            int newX = Position.X + Math.Sign(dx) * Speed;
            int newY = Position.Y + Math.Sign(dy) * Speed;

            return new Position(
                Math.Clamp(newX, 0, field.Width - 1),
                Math.Clamp(newY, 0, field.Height - 1)
            );
        }

        /// <summary>
        /// Calculates a random new position for natural movement.
        /// Used when no Lions are nearby.
        /// 1. Generate random X,Y offsets within speed range
        /// 2. Add to current position
        /// 3. Ensure new position is within field bounds
        /// </summary>
        private Position CalculateRandomPosition(GameField field)
        {
            // Try to ensure movement even near borders
            int attempts = 0;
            int newX, newY;
            do
            {
                int direction = Random.Next(GameConstants.Movement.DirectionCount);
                int dx = ((direction + 1) % GameConstants.Movement.DirectionBase + GameConstants.Movement.MinDirectionOffset) * Speed;
                int dy = ((direction / GameConstants.Movement.DirectionBase) + GameConstants.Movement.MinDirectionOffset) * Speed;
                
                newX = Position.X + dx;
                newY = Position.Y + dy;
                
                attempts++;
            } while (attempts < GameConstants.Movement.DirectionCount && // Try all directions
                    (newX == Position.X && newY == Position.Y || // Same position
                     newX < 0 || newX >= field.Width ||         // Out of bounds X
                     newY < 0 || newY >= field.Height));        // Out of bounds Y

            return new Position(
                Math.Clamp(newX, 0, field.Width - 1),
                Math.Clamp(newY, 0, field.Height - 1)
            );
        }
    }
} 