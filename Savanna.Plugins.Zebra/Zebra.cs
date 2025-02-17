using Savanna.Common.Models;
using Savanna.Common.Interfaces;
using Savanna.Plugins.Base;
using System;
using System.Linq;

namespace Savanna.Plugins.Zebra
{
    /// <summary>
    /// Represents a Zebra in the game.
    /// Similar to Antelope but faster, also tries to avoid Lions.
    /// </summary>
    public class Zebra : AnimalBase
    {
        private static readonly Random Random = new();
        private readonly ZebraConfiguration _configuration;

        public Zebra(Position position, IAnimalConfiguration configuration) 
            : base(position, configuration, new ZebraReproductionManager(position, configuration, null))
        {
            _configuration = (ZebraConfiguration)configuration;
            ((ZebraReproductionManager)ReproductionManager).UpdateHealthManager(this);
        }

        public override void Move(IGameField field)
        {
            if (!IsAlive)
            {
                return;
            }

            var oldPosition = Position;

            // Look for predators and try to escape if they're nearby
            var nearestPredator = field.GetEntitiesInRange(Position, VisionRange)
                .Where(e => (e.Symbol == _configuration.GetPredatorSymbols().lionSymbol || 
                           e.Symbol == _configuration.GetPredatorSymbols().tigerSymbol) && e.IsAlive)
                .OrderBy(e => e.Position.DistanceTo(Position))
                .FirstOrDefault();

            Position = nearestPredator != null 
                ? CalculateEscapePosition(field, nearestPredator)
                : CalculateRandomPosition(field);

            DecreaseHealth(_configuration.GetMovementCost());
        }

        private Position CalculateEscapePosition(IGameField field, IGameEntity predator)
        {
            // Calculate direction away from predator
            int dx = Position.X - predator.Position.X;
            int dy = Position.Y - predator.Position.Y;

            // Add randomness if perfectly aligned to avoid getting stuck
            if (dx == 0) dx = Random.Next(-1, 2);
            if (dy == 0) dy = Random.Next(-1, 2);

            // Move in the opposite direction of the predator at full speed
            int newX = Position.X + Math.Sign(dx) * Speed;
            int newY = Position.Y + Math.Sign(dy) * Speed;

            return field.ClampPosition(new Position(newX, newY));
        }

        private Position CalculateRandomPosition(IGameField field)
        {
            int newX = Position.X + Random.Next(-Speed, Speed + 1);
            int newY = Position.Y + Random.Next(-Speed, Speed + 1);
            return field.ClampPosition(new Position(newX, newY));
        }

        public override void PerformAction(IGameField field)
        {
            if (!IsAlive) return;

            UpdateReproductionStatus(field);
            
            if (CanReproduce)
            {
                var offspring = Reproduce(Position);
                field.AddAnimal(offspring.Symbol, offspring.Position);
                DecreaseHealth(_configuration.GetReproductionCost());
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