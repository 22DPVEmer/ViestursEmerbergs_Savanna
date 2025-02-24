using System.Text.Json;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

namespace Savanna.Services.Services
{
    public class AnimalConfigurationService
    {
        private readonly ILogger<AnimalConfigurationService> _logger;
        private Dictionary<string, AnimalConfig> _animalConfigs;

        public AnimalConfigurationService(ILogger<AnimalConfigurationService> logger)
        {
            _logger = logger;
            _animalConfigs = new Dictionary<string, AnimalConfig>();
            LoadConfigurations();
        }

        public IReadOnlyDictionary<string, AnimalConfig> GetAnimalConfigurations() => _animalConfigs;

        private void LoadConfigurations()
        {
            try
            {
                var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
                _logger.LogInformation("Looking for config.json at: {Path}", configPath);

                if (!File.Exists(configPath))
                {
                    _logger.LogError("config.json not found at {Path}", configPath);
                    throw new FileNotFoundException($"Configuration file not found at {configPath}");
                }

                var jsonContent = File.ReadAllText(configPath);
                _logger.LogInformation("Read config.json content: {Content}", jsonContent);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                };

                var config = JsonSerializer.Deserialize<ConfigRoot>(jsonContent, options);
                _logger.LogInformation("Deserialized config: {Config}", JsonSerializer.Serialize(config, options));

                if (config == null)
                {
                    _logger.LogError("Failed to deserialize config.json");
                    throw new InvalidOperationException("Failed to deserialize config.json");
                }

                if (config.Plugins == null)
                {
                    _logger.LogError("Plugins section is null in config.json");
                    throw new InvalidOperationException("Plugins section is null in config.json");
                }

                if (!config.Plugins.Any())
                {
                    _logger.LogError("No plugins found in config.json");
                    throw new InvalidOperationException("No plugins found in config.json");
                }

                _animalConfigs = config.Plugins;
                _logger.LogInformation("Successfully loaded {Count} animal configurations: {Types}", 
                    _animalConfigs.Count,
                    string.Join(", ", _animalConfigs.Values.Select(c => c.Plugin.Name)));
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error in config.json");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load animal configurations");
                _animalConfigs = new Dictionary<string, AnimalConfig>();
                throw;
            }
        }
    }

    public class ConfigRoot
    {
        [JsonPropertyName("plugins")]
        public Dictionary<string, AnimalConfig> Plugins { get; set; } = new();
    }

    public class AnimalConfig
    {
        public Configuration Configuration { get; set; } = new();
        public Movement Movement { get; set; } = new();
        public Reproduction Reproduction { get; set; } = new();
        public Hunting? Hunting { get; set; }
        public Plugin Plugin { get; set; } = new();
    }

    public class Configuration
    {
        public string Symbol { get; set; } = "";
        public int Speed { get; set; }
        public int VisionRange { get; set; }
    }

    public class Movement
    {
        public int MovementCost { get; set; }
        public int MaxMovementAttempts { get; set; }
    }

    public class Reproduction
    {
        public int RequiredConsecutiveRounds { get; set; }
        public double MinimumHealthToReproduce { get; set; }
        public int MatingDistance { get; set; }
        public double ReproductionCost { get; set; }
    }

    public class Hunting
    {
        public string[] PreySymbols { get; set; } = Array.Empty<string>();
        public int HuntingRange { get; set; }
    }

    public class Plugin
    {
        public string Name { get; set; } = "";
        public string Version { get; set; } = "";
    }
} 