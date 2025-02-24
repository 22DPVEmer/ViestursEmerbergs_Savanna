using System;
using System.IO;
using System.Text.Json;
using Savanna.Common.Models;
using Savanna.Common.Constants;
using Microsoft.Extensions.Logging;

namespace Savanna.Common.Configuration
{
    public static class PluginConfigurationLoader
    {
        private static PluginsConfig _config;
        private static ILogger _logger;

        public static void Initialize(string configPath = null, ILogger logger = null)
        {
            _logger = logger;
            
            // Use provided path or default to AppDomain.CurrentDomain.BaseDirectory
            configPath ??= Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PluginConfigurationMessages.ConfigFileName);
            
            _logger?.LogInformation("Loading plugin configuration from: {ConfigPath}", configPath);
            
            if (!File.Exists(configPath))
            {
                var error = string.Format(PluginConfigurationMessages.ConfigFileNotFound, 
                    PluginConfigurationMessages.CombinedConfigName, configPath, string.Empty);
                _logger?.LogError(error);
                throw new FileNotFoundException(error);
            }

            try
            {
                var jsonString = File.ReadAllText(configPath);
                _config = JsonSerializer.Deserialize<PluginsConfig>(jsonString)
                    ?? throw new InvalidOperationException(
                        string.Format(PluginConfigurationMessages.DeserializationFailed, 
                            PluginConfigurationMessages.CombinedConfigName));
                            
                _logger?.LogInformation("Successfully loaded plugin configuration");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to load plugin configuration");
                throw;
            }
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