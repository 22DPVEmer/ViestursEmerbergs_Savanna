using Savanna.GameEngine.Constants;
using Savanna.Common.Models;
using Savanna.Common.Interfaces;
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
        /// Finds the nearest living prey animal on the field.
        /// Uses LINQ for efficient filtering and ordering.
        /// Returns null if no prey is alive.
        /// </summary>
        private IGameEntity? FindNearestPrey(IGameField field)
        {
            return field.GetEntitiesOfTypeInRange<Antelope>(Position, VisionRange)
                .Where(a => a.IsAlive)
                .OrderBy(a => a.Position.DistanceTo(Position))
                .FirstOrDefault();
        }

        /// <summary>
        /// Implements the Lion's movement behavior.
        /// Lions will chase nearby prey within vision range,
        /// or move randomly if no prey is visible.
        /// </summary>
        protected override void PerformMove(IGameField field)
        {
            var prey = FindNearestPrey(field);
            if (prey != null && IsPreyInRange(prey))
            {
                var oldPosition = Position;
                
                // If we're already at the prey's position, catch it without moving
                if (Position.Equals(prey.Position))
                {
                    CatchPrey(prey);
                    return;
                }

                Position = CalculateChasePosition(field, prey);
                
                // Try to catch prey before applying movement cost
                if (prey.Position.DistanceTo(Position) <= GameConstants.Animal.Lion.CloseDistance)
                {
                    CatchPrey(prey);
                }
            }
            else
            {
                // Check for potential mates before moving randomly
                var nearbyMate = field.GetEntitiesOfTypeInRange<Lion>(Position, (int)GameConstants.Reproduction.MatingDistance)
                    .Where(l => l.IsAlive && l != this)
                    .FirstOrDefault();

                if (nearbyMate != null)
                {
                    // Stay close to mate if we're already within mating distance
                    var currentDistance = Position.DistanceTo(nearbyMate.Position);
                    if (currentDistance <= GameConstants.Reproduction.MatingDistance)
                    {
                        // Don't move if we're at a good distance
                        return;
                    }
                    // Move closer to mate if we're drifting apart
                    Position = CalculateChasePosition(field, nearbyMate);
                }
                else
                {
                    Position = CalculateRandomPosition(field);
                }
            }
        }

        /// <summary>
        /// Implements the Lion's special action - catching prey.
        /// If prey is within catch distance, the Lion will
        /// catch it, gaining health and causing the prey to die.
        /// </summary>
        protected override void PerformSpecialAction(IGameField field)
        {
            // Special action is now handled in PerformMove
        }

        private bool IsPreyInRange(IGameEntity prey)
        {
            return prey.Position.DistanceTo(Position) <= VisionRange;
        }

        /// <summary>
        /// Calculates the best position to chase prey:
        /// 1. Calculate direction vector to prey
        /// 2. Normalize direction using Math.Sign
        /// 3. Multiply by speed to get movement distance
        /// 4. Ensure new position is within field bounds
        /// </summary>
        private Position CalculateChasePosition(IGameField field, IGameEntity prey)
        {
            if (prey.Position.DistanceTo(Position) <= GameConstants.Animal.Lion.CloseDistance)
            {
                return prey.Position;
            }

            int dx = Math.Sign(prey.Position.X - Position.X);
            int dy = Math.Sign(prey.Position.Y - Position.Y);

            var newPosition = new Position(
                Position.X + dx * Speed,
                Position.Y + dy * Speed
            );

            return field.ClampPosition(newPosition);
        }

        /// <summary>
        /// Calculates a random new position for natural movement.
        /// Used when no prey are nearby.
        /// 1. Generate random X,Y offsets within speed range
        /// 2. Add to current position
        /// 3. Ensure new position is within field bounds
        /// </summary>
        private Position CalculateRandomPosition(IGameField field)
        {
            int newX = Position.X + Random.Next(-Speed, Speed + 1);
            int newY = Position.Y + Random.Next(-Speed, Speed + 1);
            return field.ClampPosition(new Position(newX, newY));
        }

        /// <summary>
        /// Catches prey and leaps to its position.
        /// The leap represents the final attack move.
        /// Updates the Lion's health and kills the prey.
        /// </summary>
        private void CatchPrey(IGameEntity prey)
        {
            if (prey is IHealthManageable healthManageable && healthManageable.IsAlive)
            {
                healthManageable.Die();
                IncreaseHealth(GameConstants.Health.PreyHealthValue);
            }
        }
    }
} 