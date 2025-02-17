using System;
using Savanna.GameEngine.Constants;
using Savanna.Common.Interfaces;

namespace Savanna.GameEngine.Models
{
    /// <summary>
    /// Base class for managing health-related functionality.
    /// Implements IHealthManageable to provide common health management features.
    /// </summary>
    public class HealthManager : IHealthManageable
    {
        private double _health;
        private bool _isAlive;

        /// <summary>
        /// Gets the current health value of the entity.
        /// </summary>
        public double Health => _health;

        /// <summary>
        /// Gets whether the entity is currently alive.
        /// </summary>
        public bool IsAlive => _isAlive;

        /// <summary>
        /// Initializes a new instance of the HealthManager with full health.
        /// </summary>
        public HealthManager()
        {
            _health = GameConstants.Health.InitialHealth;
            _isAlive = true;
        }

        /// <summary>
        /// Decreases the entity's health by the specified amount.
        /// If health drops below the death threshold, the entity dies.
        /// Health cannot go below minimum health value.
        /// </summary>
        public virtual void DecreaseHealth(double amount)
        {
            if (!_isAlive) return;

            _health = Math.Max(GameConstants.Health.MinimumHealth, _health - amount);
            
            if (_health <= GameConstants.Health.DeathThreshold)
            {
                Die();
            }
        }

        /// <summary>
        /// Increases the entity's health by the specified amount.
        /// Health cannot exceed initial health value.
        /// Has no effect if the entity is dead.
        /// </summary>
        public virtual void IncreaseHealth(double amount)
        {
            if (!_isAlive) return;

            _health = Math.Min(GameConstants.Health.InitialHealth, _health + amount);
        }

        /// <summary>
        /// Kills the entity, setting health to minimum and marking it as dead.
        /// This state change is irreversible.
        /// </summary>
        public virtual void Die()
        {
            _isAlive = false;
            _health = GameConstants.Health.MinimumHealth;
        }
    }
} 