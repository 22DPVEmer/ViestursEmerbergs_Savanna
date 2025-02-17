using Savanna.Common.Interfaces;
using Savanna.Plugins.Tiger.Constants;

namespace Savanna.Plugins.Tiger
{
    /// <summary>
    /// Configuration for Tiger animals.
    /// Slower but stronger with better vision range.
    /// </summary>
    public class TigerConfiguration : IAnimalConfiguration
    {
        public int Speed => TigerConstants.Configuration.Speed;
        public int VisionRange => TigerConstants.Configuration.VisionRange;
        public char Symbol => TigerConstants.Configuration.Symbol;
    }
}