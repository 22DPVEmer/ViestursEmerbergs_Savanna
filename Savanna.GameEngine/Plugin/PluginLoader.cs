using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Savanna.Common.Interfaces;
using Savanna.Common.Constants;
using Savanna.GameEngine.Constants;

namespace Savanna.GameEngine.Plugin
{
    /// <summary>
    /// Loads plugin assemblies and instantiates plugin types
    /// </summary>
    public class PluginLoader
    {
        private readonly string _pluginPath;
        private readonly Dictionary<char, IAnimalPlugin> _loadedPlugins;

        public PluginLoader(string pluginPath)
        {
            _pluginPath = pluginPath;
            _loadedPlugins = new Dictionary<char, IAnimalPlugin>();
        }

        public void LoadPlugins()
        {
            Console.WriteLine(string.Format(PluginConstants.Messages.SearchingPlugins, _pluginPath));

            // Create plugins directory if it doesn't exist
            Directory.CreateDirectory(_pluginPath);

            // Get all DLL files in the plugins directory
            var pluginFiles = Directory.GetFiles(_pluginPath, GameConstants.Plugin.DllFilePattern);
            Console.WriteLine(string.Format(PluginConstants.Messages.FoundDllFiles, pluginFiles.Length));

            foreach (var file in pluginFiles)
            {
                try
                {
                    Console.WriteLine(string.Format(PluginConstants.Messages.LoadingPluginFile, Path.GetFileName(file)));
                    var assembly = Assembly.LoadFrom(file);
                    
                    var pluginTypes = assembly.GetTypes()
                        .Where(t => typeof(IAnimalPlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                    foreach (var pluginType in pluginTypes)
                    {
                        Console.WriteLine(string.Format(PluginConstants.Messages.FoundPluginType, pluginType.Name));
                        var plugin = (IAnimalPlugin)Activator.CreateInstance(pluginType);
                        _loadedPlugins[plugin.Symbol] = plugin;
                        Console.WriteLine(string.Format(PluginConstants.Messages.LoadedPlugin, plugin.AnimalName, plugin.Symbol));
                    }
                }
                catch (Exception ex)
                {
                    // Log plugin loading error but continue with other plugins
                    Console.WriteLine(string.Format(PluginConstants.Messages.LoadingError, file, ex.Message));
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine(string.Format(PluginConstants.Messages.InnerException, ex.InnerException.Message));
                    }
                }
            }

            Console.WriteLine(string.Format(PluginConstants.Messages.TotalPluginsLoaded, _loadedPlugins.Count));
        }

        public IAnimalPlugin GetPlugin(char symbol)
        {
            return _loadedPlugins.TryGetValue(symbol, out var plugin) ? plugin : null;
        }

        public IEnumerable<IAnimalPlugin> GetAllPlugins()
        {
            return _loadedPlugins.Values;
        }
    }
}