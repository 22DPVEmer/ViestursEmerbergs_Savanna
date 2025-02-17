using System;
using Savanna.Common.Models;
using Savanna.Common.Interfaces;
using Savanna.Common.Constants;
using Savanna.Common.Configuration;

namespace Savanna.Plugins.Zebra
{
    /// <summary>
    /// Main plugin class for the Zebra animal.
    /// Implements IAnimalPlugin to integrate with the game.
    /// </summary>
    public class ZebraPlugin : IAnimalPlugin
    {
        private readonly ZebraConfiguration _configuration;
        private readonly ZebraPluginConfig _config;

        public ZebraPlugin()
        {
            try
            {
                _config = PluginConfigurationLoader.GetZebraConfig();

                // Verify the plugin name matches
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