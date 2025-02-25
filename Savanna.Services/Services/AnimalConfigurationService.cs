using System.Text.Json;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;
using Savanna.Services.Exceptions;
using Savanna.Services.Constants;
using Savanna.Services.Models;
using Savanna.Infrastructure.Constants;

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
                var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ProjectPaths.ConfigFilePath);

                if (!File.Exists(configPath))
                {
                    throw new ConfigurationException(ExceptionMessages.Configuration.FileNotFound, ProjectPaths.ConfigFilePath);
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
                    throw new ConfigurationException(ExceptionMessages.Configuration.DeserializationFailed, ProjectPaths.ConfigFilePath);
                }

                if (config.Plugins == null)
                {
                    throw new ConfigurationException(ExceptionMessages.Configuration.PluginsSectionMissing, ProjectPaths.ConfigFilePath);
                }

                if (!config.Plugins.Any())
                {
                    throw new ConfigurationException(ExceptionMessages.Configuration.NoPluginsFound, ProjectPaths.ConfigFilePath);
                }

                _animalConfigs = config.Plugins;
            }
            catch (JsonException ex)
            {
                throw new ConfigurationException(ExceptionMessages.Configuration.InvalidJsonFormat, ProjectPaths.ConfigFilePath, ex);
            }
            catch (ConfigurationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _animalConfigs = new Dictionary<string, AnimalConfig>();
                throw new ConfigurationException(ExceptionMessages.Configuration.LoadConfigurationFailed, ProjectPaths.ConfigFilePath, ex);
            }
        }
    }
} 