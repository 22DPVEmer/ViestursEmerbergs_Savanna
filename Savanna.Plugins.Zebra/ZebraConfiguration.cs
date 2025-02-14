using Savanna.Common.Interfaces;

namespace Savanna.Plugins.Zebra
{
    /// <summary>
    /// Configuration for Zebra animals.
    /// Similar to Antelope but with higher speed.
    /// </summary>
    public class ZebraConfiguration : IAnimalConfiguration
    {
        public int Speed => 3;  // Same speed as Antelope for balance
        public int VisionRange => 5;  // Same vision range as Antelope
        public char Symbol => 'Z';  // 'Z' for Zebra
    }
} 