using Savanna.Common.Interfaces;

namespace Savanna.Plugins.Tiger
{
    /// <summary>
    /// Configuration for Tiger animals.
    /// Slower but stronger with better vision range.
    /// </summary>
    public class TigerConfiguration : IAnimalConfiguration
    {
        public int Speed => 2;  // Slower but stronger
        public int VisionRange => 7;  // Better vision range like a predator
        public char Symbol => 'T';  // 'T' for Tiger
    }
}