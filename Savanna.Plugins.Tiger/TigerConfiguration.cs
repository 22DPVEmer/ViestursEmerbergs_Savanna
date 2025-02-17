using Savanna.Common.Interfaces;
using Savanna.Plugins.Tiger.Models;

namespace Savanna.Plugins.Tiger
{
    /// <summary>
    /// Configuration for Tiger animals.
    /// Predator configuration with hunting capabilities.
    /// </summary>
    public class TigerConfiguration : IAnimalConfiguration
    {
        private readonly TigerConfig _config;

        public TigerConfiguration(TigerConfig config)
        {
            _config = config;
        }

        public int Speed => _config.Configuration.Speed;
        public int VisionRange => _config.Configuration.VisionRange;
        public char Symbol => _config.Configuration.Symbol[0];

        public int GetMovementCost() => _config.Movement.MovementCost;
        public int GetMaxMovementAttempts() => _config.Movement.MaxMovementAttempts;

        public int GetRequiredConsecutiveRounds() => _config.Reproduction.RequiredConsecutiveRounds;
        public double GetMinimumHealthToReproduce() => _config.Reproduction.MinimumHealthToReproduce;
        public int GetMatingDistance() => _config.Reproduction.MatingDistance;
        public double GetReproductionCost() => _config.Reproduction.ReproductionCost;

        public char[] GetPreySymbols() => _config.Hunting.PreySymbols.Select(s => s[0]).ToArray();
        public int GetHuntingRange() => _config.Hunting.HuntingRange;
        public double GetHuntingDamage() => _config.Hunting.HuntingDamage;
        public double GetHuntingCost() => _config.Hunting.HuntingCost;
    }
}