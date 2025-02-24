using System;
using System.Linq;
using System.Collections.Generic;
using Savanna.GameEngine.Constants;
using Savanna.GameEngine.Models;
using Savanna.GameEngine.Plugin;
using Savanna.Common.Models;
using Savanna.Common.Interfaces;

namespace Savanna.GameEngine.Factory
{
    /// <summary>
    /// Factory for creating animals with proper configuration.
    /// Centralizes animal creation and configuration.
    /// Supports both built-in animals and plugin-based animals.
    /// </summary>
    public class AnimalFactory : IAnimalFactory
    {
        private readonly Dictionary<char, IAnimalConfiguration> _builtInConfigurations;
        private readonly PluginLoader _pluginLoader;

        public AnimalFactory(string pluginPath)
        {
            _builtInConfigurations = new Dictionary<char, IAnimalConfiguration>
            {
                { GameConstants.Animal.Antelope.Symbol, new AnimalConfiguration(
                    GameConstants.Animal.Antelope.Speed,
                    GameConstants.Animal.Antelope.VisionRange,
                    GameConstants.Animal.Antelope.Symbol)
                },
                { GameConstants.Animal.Lion.Symbol, new AnimalConfiguration(
                    GameConstants.Animal.Lion.Speed,
                    GameConstants.Animal.Lion.VisionRange,
                    GameConstants.Animal.Lion.Symbol)
                }
            };

            _pluginLoader = new PluginLoader(pluginPath);
            _pluginLoader.LoadPlugins();
        }

        public IGameEntity CreateAnimal(char type, Position position)
        {
            // First try to create from plugins
            var plugin = _pluginLoader.GetPlugin(type);
            if (plugin != null && plugin.IsCompatible)
            {
                return plugin.CreateAnimal(position);
            }

            // Fall back to built-in animals if no plugin found
            if (!_builtInConfigurations.ContainsKey(type))
            {
                throw new ArgumentException(string.Format(AnimalFactoryConstants.Messages.UnknownAnimalType, type));
            }

            return type switch
            {
                var t when t == GameConstants.Animal.Antelope.Symbol => 
                    new Antelope(position, _builtInConfigurations[t]),
                var t when t == GameConstants.Animal.Lion.Symbol => 
                    new Lion(position, _builtInConfigurations[t]),
                _ => throw new ArgumentException(string.Format(AnimalFactoryConstants.Messages.UnhandledAnimalType, type))
            };
        }

        /// <summary>
        /// Gets all available animal types, including both built-in and plugin-based animals
        /// </summary>
        public IEnumerable<char> GetAvailableAnimalTypes()
        {
            var builtInTypes = _builtInConfigurations.Keys;
            var pluginTypes = _pluginLoader.GetAllPlugins().Select(p => p.Symbol);
            return builtInTypes.Concat(pluginTypes).Distinct();
        }
    }

    /// <summary>
    /// Configuration record for animal properties.
    /// Implements IAnimalConfiguration for use in factory.
    /// </summary>
    public record AnimalConfiguration(int Speed, int VisionRange, char Symbol) : IAnimalConfiguration;
}