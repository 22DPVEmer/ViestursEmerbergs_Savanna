using System.Text.Json.Serialization;

namespace Savanna.Services.Models
{
    public class ConfigRoot
    {
        [JsonPropertyName("plugins")]
        public Dictionary<string, AnimalConfig> Plugins { get; set; } = new();
    }
} 