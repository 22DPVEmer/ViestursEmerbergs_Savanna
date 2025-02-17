using System;
using System.IO;
using System.Text.Json;
using Savanna.Common.Models;
using Savanna.Common.Constants;

namespace Savanna.Common.Configuration
{
    public static class PluginConfigurationLoader
    {
        private static PluginsConfig _config;

        public static void Initialize()
        {
            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PluginConfigurationMessages.ConfigFileName);
            
            if (!File.Exists(configPath))
            {
                throw new FileNotFoundException(
                    string.Format(PluginConfigurationMessages.ConfigFileNotFound, 
                        PluginConfigurationMessages.CombinedConfigName, configPath, string.Empty));
            }

            var jsonString = File.ReadAllText(configPath);
            _config = JsonSerializer.Deserialize<PluginsConfig>(jsonString)
                ?? throw new InvalidOperationException(
                    string.Format(PluginConfigurationMessages.DeserializationFailed, 
                        PluginConfigurationMessages.CombinedConfigName));
        }

        public static TigerPluginConfig GetTigerConfig()
        {
            EnsureInitialized();
            return _config.Plugins.Tiger;
        }

        public static ZebraPluginConfig GetZebraConfig()
        {
            EnsureInitialized();
            return _config.Plugins.Zebra;
        }

        private static void EnsureInitialized()
        {
            if (_config == null)
            {
                throw new InvalidOperationException(PluginConfigurationMessages.NotInitialized);
            }
        }
    }
} 