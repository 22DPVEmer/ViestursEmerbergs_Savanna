using Savanna.Common.Interfaces;
using Savanna.Plugins.Zebra.Constants;

namespace Savanna.Plugins.Zebra
{
    /// <summary>
    /// Configuration for Zebra animals.
    /// Similar to Antelope but with higher speed.
    /// </summary>
    public class ZebraConfiguration : IAnimalConfiguration
    {
        public int Speed => ZebraConstants.Configuration.Speed;
        public int VisionRange => ZebraConstants.Configuration.VisionRange;
        public char Symbol => ZebraConstants.Configuration.Symbol;
    }
} 