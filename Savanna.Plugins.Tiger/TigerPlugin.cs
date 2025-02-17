using System;
using Savanna.Common.Models;
using Savanna.Common.Interfaces;
using Savanna.Common.Constants;
using Savanna.Common.Configuration;

namespace Savanna.Plugins.Tiger
{
    /// <summary>
    /// Main plugin class for the Tiger animal.
    /// Implements IAnimalPlugin to integrate with the game.
    /// </summary>
    public class TigerPlugin : IAnimalPlugin
    {
        private readonly TigerConfiguration _configuration;
        private readonly TigerPluginConfig _config;

        public TigerPlugin()
        {
            try
            {
                _config = PluginConfigurationLoader.GetTigerConfig();

                // Verify the plugin name matches
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