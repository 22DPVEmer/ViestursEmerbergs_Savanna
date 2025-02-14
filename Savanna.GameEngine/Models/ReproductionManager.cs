using Savanna.GameEngine.Constants;
using Savanna.Common.Models;
using Savanna.Common.Interfaces;

namespace Savanna.GameEngine.Models
{
    /// <summary>
    /// Base class for managing reproduction-related functionality
    /// Implements IReproducible to provide common reproduction features
    /// </summary>
    public abstract class ReproductionManager : IReproducible
    {
        private int _consecutiveRoundsNearMate;
        private readonly IHealthManageable _healthManager;

        public int ConsecutiveRoundsNearMate => _consecutiveRoundsNearMate;
        
        public bool CanReproduce => 
            _consecutiveRoundsNearMate >= GameConstants.Reproduction.RequiredConsecutiveRounds &&
            _healthManager.Health >= GameConstants.Reproduction.MinimumHealthToReproduce &&
            _healthManager.IsAlive;

        protected ReproductionManager(IHealthManageable healthManager)
        {
            _healthManager = healthManager;
            _consecutiveRoundsNearMate = 0;
        }

        public void UpdateReproductionStatus(IGameField field)
        {
            if (!_healthManager.IsAlive || 
                _healthManager.Health < GameConstants.Reproduction.MinimumHealthToReproduce)
            {
                _consecutiveRoundsNearMate = 0;
                return;
            }

            bool isNearMate = IsNearMate(field);
            if (isNearMate)
            {
                _consecutiveRoundsNearMate++;
            }
            else
            {
                _consecutiveRoundsNearMate = 0;
            }
        }

        public abstract IGameEntity Reproduce(Position position);

        protected abstract bool IsNearMate(IGameField field);
    }
} 