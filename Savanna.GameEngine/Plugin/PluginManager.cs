using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Savanna.Common.Interfaces;
using Savanna.Common.Models;
using Savanna.Common.Constants;

namespace Savanna.GameEngine.Plugin
{
    /// <summary>
    /// Manages the lifecycle of animal plugins in the game
    /// </summary>
    public class PluginManager
    {
        private readonly PluginLoader _pluginLoader;
        private readonly Dictionary<char, IAnimalPlugin> _activePlugins;
        private readonly string _pluginDirectory;

        public PluginManager(string pluginDirectory)
        {
            _pluginDirectory = pluginDirectory;
            _pluginLoader = new PluginLoader(pluginDirectory);
            _activePlugins = new Dictionary<char, IAnimalPlugin>();
        }

        /// <summary>
        /// Initializes the plugin system and loads available plugins
        /// </summary>
        public void Initialize()
        {
            // Ensure plugin directory exists
            Directory.CreateDirectory(_pluginDirectory);

            // Load plugins
            _pluginLoader.LoadPlugins();

            // Register active plugins
            foreach (var plugin in _pluginLoader.GetAllPlugins())
            {
                if (plugin.IsCompatible)
                {
                    _activePlugins[plugin.Symbol] = plugin;
                    Console.WriteLine(string.Format(PluginConstants.Messages.PluginRegistered, plugin.AnimalName, plugin.Symbol));
                }
                else
                {
                    Console.WriteLine(string.Format(PluginConstants.Messages.IncompatiblePlugin, plugin.AnimalName));
                }
            }
        }

        /// <summary>
        /// Gets all available animal symbols (including plugins)
        /// </summary>
        public IEnumerable<char> GetAvailableAnimalSymbols()
        {
            return _activePlugins.Keys;
        }

        /// <summary>
        /// Creates an animal instance from a plugin
        /// </summary>
        public IGameEntity CreateAnimal(char symbol, Position position)
        {
            if (_activePlugins.TryGetValue(symbol, out var plugin))
            {
                return plugin.CreateAnimal(position);
            }

            throw new ArgumentException(string.Format(PluginConstants.Messages.PluginNotFound, symbol));
        }

        /// <summary>
        /// Checks if a plugin exists for the given symbol
        /// </summary>
        public bool HasPlugin(char symbol)
        {
            return _activePlugins.ContainsKey(symbol);
        }

        /// <summary>
        /// Gets plugin information for display
        /// </summary>
        public IEnumerable<(string Name, char Symbol, string Version)> GetPluginInfo()
        {
            return _activePlugins.Values.Select(p => (p.AnimalName, p.Symbol, p.Version));
        }
    }
}