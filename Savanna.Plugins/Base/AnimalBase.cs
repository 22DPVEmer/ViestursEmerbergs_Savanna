using Savanna.Common.Models;
using Savanna.Common.Interfaces;
using System;

namespace Savanna.Plugins.Base
{
    /// <summary>
    /// Base class for implementing animals in plugins
    /// </summary>
    public abstract class AnimalBase : IGameEntity, IMovable, IActionable, IHealthManageable, IReproducible
    {
        private readonly IAnimalConfiguration _configuration;
        private double _health = 100;
        private IReproducible _reproductionManager;

        protected AnimalBase(Position position, IAnimalConfiguration configuration, IReproducible reproductionManager)
        {
            Position = position;
            _configuration = configuration;
            _reproductionManager = reproductionManager;
        }

        public IReproducible ReproductionManager
        {
            get => _reproductionManager;
            set => _reproductionManager = value ?? throw new ArgumentNullException(nameof(value));
        }

        public Position Position { get; set; }
        public char Symbol => _configuration.Symbol;
        public bool IsAlive => _health > 0;
        public double Health => _health;
        public int Speed => _configuration.Speed;
        public int VisionRange => _configuration.VisionRange;
        public int ConsecutiveRoundsNearMate => _reproductionManager.ConsecutiveRoundsNearMate;
        public bool CanReproduce => _reproductionManager.CanReproduce;

        public virtual void DecreaseHealth(double amount)
        {
            if (!IsAlive) return;
            var oldHealth = _health;
            _health = Math.Max(0, _health - amount);
            OnHealthChanged(oldHealth, _health);
        }

        public virtual void IncreaseHealth(double amount)
        {
            if (!IsAlive) return;
            var oldHealth = _health;
            _health = Math.Min(100, _health + amount);
            OnHealthChanged(oldHealth, _health);
        }

        public virtual void Die()
        {
            _health = 0;
            OnDeath();
        }

        protected virtual void OnHealthChanged(double oldHealth, double newHealth) { }
        protected virtual void OnDeath() { }

        public abstract void Move(IGameField field);
        public virtual void PerformAction(IGameField field)
        {
            if (!IsAlive) return;

            UpdateReproductionStatus(field);
            
            // Only reproduce if the field has space
            if (CanReproduce && !field.IsAtCapacity())
            {
                var offspring = Reproduce(Position);
                field.AddAnimal(offspring.Symbol, offspring.Position);
                DecreaseHealth(20); // Default reproduction cost
            }
        }

        public void UpdateReproductionStatus(IGameField field)
        {
            _reproductionManager.UpdateReproductionStatus(field);
        }

        public IGameEntity Reproduce(Position position)
        {
            return _reproductionManager.Reproduce(position);
        }
    }
}