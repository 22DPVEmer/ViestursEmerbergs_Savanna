using Savanna.Common.Models;
using Savanna.Common.Interfaces;
using System;
using System.Linq;

namespace Savanna.Plugins.Zebra
{
    /// <summary>
    /// Handles reproduction logic specific to Zebras.
    /// Similar to Antelope reproduction.
    /// </summary>
    internal class ZebraReproductionManager : IReproducible
    {
        private readonly Position _parentPosition;
        private readonly ZebraConfiguration _configuration;
        private IHealthManageable _healthManager;
        private int _consecutiveRoundsNearMate;

        public int ConsecutiveRoundsNearMate => _consecutiveRoundsNearMate;
        public bool CanReproduce => 
            _consecutiveRoundsNearMate >= _configuration.GetRequiredConsecutiveRounds() &&
            _healthManager?.Health >= _configuration.GetMinimumHealthToReproduce();

        public ZebraReproductionManager(Position position, IAnimalConfiguration configuration, IHealthManageable healthManager)
        {
            _parentPosition = position;
            _configuration = (ZebraConfiguration)configuration;
            _healthManager = healthManager;
            _consecutiveRoundsNearMate = 0;
        }

        public void UpdateHealthManager(IHealthManageable healthManager)
        {
            _healthManager = healthManager ?? throw new ArgumentNullException(nameof(healthManager));
        }

        public void UpdateReproductionStatus(IGameField field)
        {
            if (!_healthManager.IsAlive || _healthManager.Health < _configuration.GetMinimumHealthToReproduce())
            {
                _consecutiveRoundsNearMate = 0;
                return;
            }

            bool isNearMate = field.GetEntitiesInRange(_parentPosition, _configuration.GetMatingDistance())
                .Where(z => z.Symbol == _configuration.Symbol && z.IsAlive)
                .Any();

            if (isNearMate)
            {
                _consecutiveRoundsNearMate++;
            }
            else
            {
                _consecutiveRoundsNearMate = 0;
            }
        }

        public IGameEntity Reproduce(Position position)
        {
            var newPosition = new Position(
                position.X + Random.Shared.Next(-1, 2),
                position.Y + Random.Shared.Next(-1, 2)
            );
            
            return new Zebra(newPosition, _configuration);
        }
    }
}