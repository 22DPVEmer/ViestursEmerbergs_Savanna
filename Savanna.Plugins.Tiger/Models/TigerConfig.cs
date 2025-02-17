using System.Text.Json.Serialization;

namespace Savanna.Plugins.Tiger.Models
{
    public class TigerConfig
    {
        [JsonPropertyName("configuration")]
        public ConfigurationSection Configuration { get; set; }

        [JsonPropertyName("movement")]
        public MovementSection Movement { get; set; }

        [JsonPropertyName("reproduction")]
        public ReproductionSection Reproduction { get; set; }

        [JsonPropertyName("hunting")]
        public HuntingSection Hunting { get; set; }

        [JsonPropertyName("plugin")]
        public PluginSection Plugin { get; set; }
    }

    public class ConfigurationSection
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        [JsonPropertyName("speed")]
        public int Speed { get; set; }

        [JsonPropertyName("visionRange")]
        public int VisionRange { get; set; }
    }

    public class MovementSection
    {
        [JsonPropertyName("movementCost")]
        public int MovementCost { get; set; }

        [JsonPropertyName("maxMovementAttempts")]
        public int MaxMovementAttempts { get; set; }
    }

    public class ReproductionSection
    {
        [JsonPropertyName("requiredConsecutiveRounds")]
        public int RequiredConsecutiveRounds { get; set; }

        [JsonPropertyName("minimumHealthToReproduce")]
        public double MinimumHealthToReproduce { get; set; }

        [JsonPropertyName("matingDistance")]
        public int MatingDistance { get; set; }

        [JsonPropertyName("reproductionCost")]
        public double ReproductionCost { get; set; }
    }

    public class HuntingSection
    {
        [JsonPropertyName("preySymbols")]
        public string[] PreySymbols { get; set; }

        [JsonPropertyName("huntingRange")]
        public int HuntingRange { get; set; }

        [JsonPropertyName("huntingDamage")]
        public double HuntingDamage { get; set; }

        [JsonPropertyName("huntingCost")]
        public double HuntingCost { get; set; }
    }

    public class PluginSection
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }
    }
} 