using System;
using System.IO;
using System.Text.Json;
using Savanna.Common.Models;
using Savanna.Common.Interfaces;
using Savanna.Common.Constants;
using Savanna.Plugins.Zebra.Models;

namespace Savanna.Plugins.Zebra
{
    /// <summary>
    /// Main plugin class for the Zebra animal.
    /// Implements IAnimalPlugin to integrate with the game.
    /// </summary>
    public class ZebraPlugin : IAnimalPlugin
    {
        private readonly ZebraConfiguration _configuration;
        private readonly ZebraConfig _config;

        public ZebraPlugin()
        {
            try
            {
                // Try the plugin-specific directory first
                var pluginPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, 
                    PluginConfigurationMessages.PluginsDirectory, 
                    PluginNames.Zebra, 
                    PluginConfigurationMessages.ConfigFileName);
                
                // Fallback to the root directory if not found in plugin directory
                var rootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, 
                    PluginConfigurationMessages.ConfigFileName);
                
                var configPath = File.Exists(pluginPath) ? pluginPath : rootPath;
                
                if (!File.Exists(configPath))
                {
                    throw new FileNotFoundException(
                        string.Format(PluginConfigurationMessages.ConfigFileNotFound, PluginNames.Zebra, pluginPath, rootPath));
                }

                var jsonString = File.ReadAllText(configPath);
                _config = JsonSerializer.Deserialize<ZebraConfig>(jsonString) 
                    ?? throw new InvalidOperationException(
                        string.Format(PluginConfigurationMessages.DeserializationFailed, PluginNames.Zebra));

                // Verify the plugin name in config matches the expected name
                if (_config.Plugin.Name != PluginNames.Zebra)
                {
                    throw new InvalidOperationException(
                        string.Format(PluginConfigurationMessages.PluginNameMismatch, _config.Plugin.Name, PluginNames.Zebra));
                }

                _configuration = new ZebraConfiguration(_config);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    string.Format(PluginConfigurationMessages.InitializationFailed, PluginNames.Zebra, ex.Message), ex);
            }
        }

        public string AnimalName => _config.Plugin.Name;
        public char Symbol => _config.Configuration.Symbol[0];
        public IAnimalConfiguration Configuration => _configuration;
        public string Version => _config.Plugin.Version;
        public bool IsCompatible => true;

        public IGameEntity CreateAnimal(Position position)
        {
            return new Zebra(position, Configuration);
        }
    }
}