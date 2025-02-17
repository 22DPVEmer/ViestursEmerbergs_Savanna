using Savanna.Common.Interfaces;
using Savanna.Plugins.Zebra.Models;

namespace Savanna.Plugins.Zebra
{
    /// <summary>
    /// Configuration for Zebra animals.
    /// Similar to Antelope but with higher speed.
    /// </summary>
    public class ZebraConfiguration : IAnimalConfiguration
    {
        private readonly ZebraConfig _config;

        public ZebraConfiguration(ZebraConfig config)
        {
            _config = config;
        }

        public int Speed => _config.Configuration.Speed;
        public int VisionRange => _config.Configuration.VisionRange;
        public char Symbol => _config.Configuration.Symbol[0];

        public (char lionSymbol, char tigerSymbol) GetPredatorSymbols() =>
            (_config.Symbols.LionSymbol[0], _config.Symbols.TigerSymbol[0]);

        public int GetMovementCost() => _config.Movement.MovementCost;

        public double GetReproductionCost() => _config.Reproduction.ReproductionCost;

        public int GetRequiredConsecutiveRounds() => _config.Reproduction.RequiredConsecutiveRounds;

        public double GetMinimumHealthToReproduce() => _config.Reproduction.MinimumHealthToReproduce;

        public int GetMatingDistance() => _config.Reproduction.MatingDistance;
    }
} 