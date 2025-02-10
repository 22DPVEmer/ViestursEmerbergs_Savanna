using Savanna.GameEngine.Constants;
using Savanna.GameEngine.Interfaces;
using System;
using System.Linq;

namespace Savanna.GameEngine.Models
{
    /// <summary>
    /// Represents a Lion in the game.
    /// Lions are predators that hunt Antelopes.
    /// They move slower but have better vision range than Antelopes.
    /// </summary>
    public class Lion : Animal
    {
        // Shared Random instance to prevent same-tick values
        private static readonly Random Random = new Random();
        
        private const double CLOSE_DISTANCE = 1.5;

        /// <summary>
        /// Creates a new Lion at the specified position with given configuration.
        /// </summary>
        public Lion(Position position, IAnimalConfiguration configuration) 
            : base(position, configuration)
        {
        }

        /// <summary>
        /// Creates a reproduction manager specific to Lions.
        /// </summary>
        protected override ReproductionManager CreateReproductionManager()
        {
            return new LionReproductionManager(this, this);
        }

        /// <summary>
        /// Creates a new Lion offspring at the specified position.
        /// Inherits configuration from the parent.
        /// </summary>
        public override IGameEntity Reproduce(Position position)
        {
            return new Lion(position, GetConfiguration());
        }

        /// <summary>
        /// Implements the Lion's movement behavior.
        /// Lions will chase nearby Antelopes within vision range,
        /// or move randomly if no Antelopes are visible.
        /// </summary>
        protected override void PerformMove(GameField field)
        {
            var nearestAntelope = FindNearestAntelope(field);
            if (nearestAntelope == null || Position.DistanceTo(nearestAntelope.Position) > VisionRange) 
            {
                Position = CalculateRandomPosition(field);
                return;
            }

            // Always move towards Antelope if we can see it
            Position = CalculateChasePosition(field, nearestAntelope);
        }

        /// <summary>
        /// Implements the Lion's special action - catching Antelopes.
        /// If an Antelope is within catch distance, the Lion will
        /// catch it, gaining health and causing the Antelope to die.
        /// </summary>
        protected override void PerformSpecialAction(GameField field)
        {
            var nearestAntelope = FindNearestAntelope(field);
            if (nearestAntelope != null && Position.DistanceTo(nearestAntelope.Position) <= GameConstants.Animal.Lion.CatchDistance)
            {
                CatchAntelope(nearestAntelope);
            }
        }

        /// <summary>
        /// Finds the nearest living Antelope on the field.
        /// Uses LINQ for efficient filtering and ordering.
        /// Returns null if no Antelopes are alive.
        /// </summary>
        private Antelope? FindNearestAntelope(GameField field) =>
            field.Animals
                .OfType<Antelope>()
                .Where(a => a.IsAlive)
                .OrderBy(a => a.Position.DistanceTo(Position))
                .FirstOrDefault();

        /// <summary>
        /// Calculates the best position to chase an Antelope:
        /// 1. Calculate direction vector to Antelope
        /// 2. Normalize direction using Math.Sign
        /// 3. Multiply by speed to get movement distance
        /// 4. Ensure new position is within field bounds
        /// </summary>
        private Position CalculateChasePosition(GameField field, Antelope antelope)
        {
            double distance = Position.DistanceTo(antelope.Position);
            
            // When very close, move directly to antelope
            if (distance <= CLOSE_DISTANCE)
            {
                return antelope.Position;
            }

            // Calculate direction to antelope
            int dx = antelope.Position.X - Position.X;
            int dy = antelope.Position.Y - Position.Y;

            // Always move towards the antelope
            int newX = Position.X + Math.Sign(dx) * Speed;
            int newY = Position.Y + Math.Sign(dy) * Speed;

            // Ensure we stay within bounds
            return new Position(
                Math.Clamp(newX, 0, field.Width - 1),
                Math.Clamp(newY, 0, field.Height - 1)
            );
        }

        /// <summary>
        /// Calculates a random new position for natural movement.
        /// Used when no Antelopes are nearby.
        /// 1. Generate random X,Y offsets within speed range
        /// 2. Add to current position
        /// 3. Ensure new position is within field bounds
        /// </summary>
        private Position CalculateRandomPosition(GameField field)
        {
            // Ensure some movement always happens
            int newX, newY;
            do
            {
                newX = Position.X + Random.Next(-Speed, Speed + 1);
                newY = Position.Y + Random.Next(-Speed, Speed + 1);
            } while (newX == Position.X && newY == Position.Y);

            return new Position(
                Math.Clamp(newX, 0, field.Width - 1),
                Math.Clamp(newY, 0, field.Height - 1)
            );
        }

        /// <summary>
        /// Catches an Antelope and leaps to its position.
        /// The leap represents the final attack move.
        /// Updates the Lion's health and kills the Antelope.
        /// </summary>
        private void CatchAntelope(Antelope antelope)
        {
            Position = antelope.Position;
            antelope.Die();
            IncreaseHealth(GameConstants.Health.PreyHealthValue);
        }
    }
} 