using Savanna.Common.Models;
using Savanna.Common.Interfaces;
using Savanna.Plugins.Tiger.Constants;
using System;
using System.Linq;

namespace Savanna.Plugins.Tiger
{
    /// <summary>
    /// Handles reproduction logic specific to Tigers.
    /// Similar to Antelope reproduction.
    /// </summary>
    internal class TigerReproductionManager : IReproducible
    {
        private readonly Position _parentPosition;
        private readonly IAnimalConfiguration _configuration;
        private IHealthManageable? _healthManager;
        private int _consecutiveRoundsNearMate;

        public int ConsecutiveRoundsNearMate => _consecutiveRoundsNearMate;
        public bool CanReproduce => 
            _consecutiveRoundsNearMate >= TigerConstants.Reproduction.RequiredConsecutiveRounds &&
            _healthManager?.Health >= TigerConstants.Reproduction.MinimumHealthToReproduce;

        public TigerReproductionManager(Position position, IAnimalConfiguration configuration, IHealthManageable healthManager)
        {
            _parentPosition = position;
            _configuration = configuration;
            _healthManager = healthManager;
            _consecutiveRoundsNearMate = 0;
        }

        public void UpdateHealthManager(IHealthManageable healthManager)
        {
            _healthManager = healthManager ?? throw new ArgumentNullException(nameof(healthManager));
        }

        public void UpdateReproductionStatus(IGameField field)
        {
            if (!_healthManager.IsAlive || _healthManager.Health < TigerConstants.Reproduction.MinimumHealthToReproduce)
            {
                _consecutiveRoundsNearMate = 0;
                return;
            }

            bool isNearMate = field.GetEntitiesInRange(_parentPosition, TigerConstants.Reproduction.MatingDistance)
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
            
            var configuration = new TigerConfiguration();
            return new Tiger(newPosition, configuration);
        }
    }
}