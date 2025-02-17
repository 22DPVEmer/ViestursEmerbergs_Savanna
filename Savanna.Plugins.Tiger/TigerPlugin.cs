using System;
using System.IO;
using System.Text.Json;
using Savanna.Common.Models;
using Savanna.Common.Interfaces;
using Savanna.Common.Constants;
using Savanna.Plugins.Tiger.Models;

namespace Savanna.Plugins.Tiger
{
    /// <summary>
    /// Main plugin class for the Tiger animal.
    /// Implements IAnimalPlugin to integrate with the game.
    /// </summary>
    public class TigerPlugin : IAnimalPlugin
    {
        private readonly TigerConfiguration _configuration;
        private readonly TigerConfig _config;

        public TigerPlugin()
        {
            try
            {
                // Try the plugin-specific directory first
                var pluginPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, 
                    PluginConfigurationMessages.PluginsDirectory, 
                    PluginNames.Tiger, 
                    PluginConfigurationMessages.ConfigFileName);
                
                // Fallback to the root directory if not found in plugin directory
                var rootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, 
                    PluginConfigurationMessages.ConfigFileName);
                
                var configPath = File.Exists(pluginPath) ? pluginPath : rootPath;
                
                if (!File.Exists(configPath))
                {
                    throw new FileNotFoundException(
                        string.Format(PluginConfigurationMessages.ConfigFileNotFound, PluginNames.Tiger, pluginPath, rootPath));
                }

                var jsonString = File.ReadAllText(configPath);
                _config = JsonSerializer.Deserialize<TigerConfig>(jsonString) 
                    ?? throw new InvalidOperationException(
                        string.Format(PluginConfigurationMessages.DeserializationFailed, PluginNames.Tiger));

                // Verify the plugin name in config matches the expected name
                if (_config.Plugin.Name != PluginNames.Tiger)
                {
                    throw new InvalidOperationException(
                        string.Format(PluginConfigurationMessages.PluginNameMismatch, _config.Plugin.Name, PluginNames.Tiger));
                }

                _configuration = new TigerConfiguration(_config);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    string.Format(PluginConfigurationMessages.InitializationFailed, PluginNames.Tiger, ex.Message), ex);
            }
        }

        public string AnimalName => _config.Plugin.Name;
        public char Symbol => _config.Configuration.Symbol[0];
        public IAnimalConfiguration Configuration => _configuration;
        public string Version => _config.Plugin.Version;
        public bool IsCompatible => true;

        public IGameEntity CreateAnimal(Position position)
        {
            return new Tiger(position, Configuration);
        }
    }
}