using Savanna.Common.Models;
using Savanna.Common.Interfaces;
using Savanna.Plugins.Base;
using Savanna.Plugins.Tiger.Constants;
using System;
using System.Linq;

namespace Savanna.Plugins.Tiger
{
    /// <summary>
    /// Represents a Tiger in the game.
    /// Similar to Antelope but faster, also tries to avoid Lions.
    /// </summary>
    public class Tiger : AnimalBase
    {
        private static readonly Random Random = new();
        
        public Tiger(Position position, IAnimalConfiguration configuration) 
            : base(position, configuration, new TigerReproductionManager(position, configuration, null))
        {
            ((TigerReproductionManager)ReproductionManager).UpdateHealthManager(this);
        }

        /// <summary>
        /// Finds the nearest prey that a Tiger can hunt:
        /// - Excludes Lions (they're too dangerous)
        /// - Excludes other Tigers (no cannibalism)
        /// - Only includes living entities
        /// </summary>
        private IGameEntity? FindNearestPrey(IGameField field)
        {
            return field.GetEntitiesInRange(Position, VisionRange)
                .Where(entity =>
                    entity.Symbol != TigerConstants.Symbols.LionSymbol &&  // Don't attack lions
                    (entity.Symbol == TigerConstants.Symbols.ZebraSymbol || 
                     entity.Symbol == TigerConstants.Symbols.AntelopeSymbol) &&  // Attack Zebras or Antelopes
                    entity.IsAlive)                     // Only target living entities
                .OrderBy(entity => entity.Position.DistanceTo(Position))
                .FirstOrDefault();
        }

        public override void Move(IGameField field)
        {
            if (!IsAlive) return;

            var prey = FindNearestPrey(field);
            if (prey != null)
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
                if (prey.Position.DistanceTo(Position) <= TigerConstants.Action.CatchDistance)
                {
                    CatchPrey(prey);
                }
            }
            else
            {
                Position = CalculateRandomPosition(field);
            }

            DecreaseHealth(TigerConstants.Movement.MovementCost);
        }

        private Position CalculateChasePosition(IGameField field, IGameEntity prey)
        {
            if (prey.Position.DistanceTo(Position) <= TigerConstants.Action.CatchDistance)
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

        private Position CalculateRandomPosition(IGameField field)
        {
            int newX = Position.X + Random.Next(-Speed, Speed + 1);
            int newY = Position.Y + Random.Next(-Speed, Speed + 1);
            return field.ClampPosition(new Position(newX, newY));
        }

        private void CatchPrey(IGameEntity prey)
        {
            if (prey is IHealthManageable healthManageable && healthManageable.IsAlive)
            {
                healthManageable.Die();
                IncreaseHealth(TigerConstants.Action.PreyHealthValue);
            }
        }

        public override void PerformAction(IGameField field)
        {
            if (!IsAlive) return;

            UpdateReproductionStatus(field);

            if (CanReproduce)
            {
                var offspring = Reproduce(Position);
                field.AddAnimal(offspring.Symbol, offspring.Position);
                DecreaseHealth(TigerConstants.Reproduction.ReproductionCost);
            }
        }

        protected override void OnHealthChanged(double oldHealth, double newHealth)
        {
        }

        protected override void OnDeath()
        {
        }
    }
}