using System.Text.Json;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using Savanna.Services.Exceptions;
using Savanna.Services.Constants;

namespace Savanna.Services.Services
{
    public class AnimalConfigurationService
    {
        private readonly ILogger<AnimalConfigurationService> _logger;
        private Dictionary<string, AnimalConfig> _animalConfigs;
        private const string CONFIG_FILE_NAME = "config.json";

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
                var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CONFIG_FILE_NAME);

                if (!File.Exists(configPath))
                {
                    throw new ConfigurationException(ExceptionMessages.Configuration.FileNotFound, CONFIG_FILE_NAME);
                }

                var jsonContent = File.ReadAllText(configPath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                };

                var config = JsonSerializer.Deserialize<ConfigRoot>(jsonContent, options);

                if (config == null)
                {
                    throw new ConfigurationException(ExceptionMessages.Configuration.DeserializationFailed, CONFIG_FILE_NAME);
                }

                if (config.Plugins == null)
                {
                    throw new ConfigurationException(ExceptionMessages.Configuration.PluginsSectionMissing, CONFIG_FILE_NAME);
                }

                if (!config.Plugins.Any())
                {
                    throw new ConfigurationException(ExceptionMessages.Configuration.NoPluginsFound, CONFIG_FILE_NAME);
                }

                _animalConfigs = config.Plugins;
            }
            catch (JsonException ex)
            {
                throw new ConfigurationException(ExceptionMessages.Configuration.InvalidJsonFormat, CONFIG_FILE_NAME, ex);
            }
            catch (ConfigurationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _animalConfigs = new Dictionary<string, AnimalConfig>();
                throw new ConfigurationException(ExceptionMessages.Configuration.LoadConfigurationFailed, CONFIG_FILE_NAME, ex);
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