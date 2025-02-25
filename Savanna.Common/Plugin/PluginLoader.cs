using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Savanna.Common.Interfaces;
using Savanna.Common.Constants;

namespace Savanna.Common.Plugin
{
    /// <summary>
    /// Unified plugin loader that handles loading and managing game plugins
    /// Can be used by both web and console applications
    /// </summary>
    public class PluginLoader
    {
        private readonly ILogger? _logger;
        private readonly string _pluginPath;
        private readonly Dictionary<char, IAnimalPlugin> _loadedPlugins;
        private readonly Dictionary<string, Assembly> _loadedAssemblies;

        /// <summary>
        /// Initializes a new instance of the PluginLoader class
        /// </summary>
        /// <param name="pluginPath">Path to the plugins directory. If null, uses default location</param>
        /// <param name="logger">Optional logger for recording plugin operations</param>
        public PluginLoader(string? pluginPath = null, ILogger? logger = null)
        {
            _logger = logger;
            _pluginPath = pluginPath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PluginConstants.Paths.PluginDirectory);
            _loadedPlugins = new Dictionary<char, IAnimalPlugin>();
            _loadedAssemblies = new Dictionary<string, Assembly>();
            
            Directory.CreateDirectory(_pluginPath); // Ensure plugin directory exists
        }

        /// <summary>
        /// Loads all plugin assemblies and instantiates available plugins
        /// </summary>
        public void LoadPlugins()
        {
            LogInfo(PluginConstants.Messages.SearchingPlugins, _pluginPath);

            try
            {
                var pluginFiles = Directory.GetFiles(_pluginPath, PluginConstants.Paths.DllSearchPattern);
                LogInfo(PluginConstants.Messages.FoundDllFiles, pluginFiles.Length);

                foreach (var file in pluginFiles)
                {
                    try
                    {
                        var fileName = Path.GetFileName(file);
                        LogInfo(PluginConstants.Messages.LoadingPluginFile, fileName);
                        
                        var assemblyName = Path.GetFileNameWithoutExtension(file);
                        if (!_loadedAssemblies.ContainsKey(assemblyName))
                        {
                            var assembly = Assembly.LoadFrom(file);
                            _loadedAssemblies[assemblyName] = assembly;

                            // Look for and instantiate any animal plugins
                            var pluginTypes = assembly.GetTypes()
                                .Where(t => typeof(IAnimalPlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                            foreach (var pluginType in pluginTypes)
                            {
                                try
                                {
                                    LogInfo(PluginConstants.Messages.FoundPluginType, pluginType.Name);
                                    var plugin = (IAnimalPlugin)Activator.CreateInstance(pluginType);
                                    
                                    if (plugin != null && plugin.IsCompatible)
                                    {
                                        _loadedPlugins[plugin.Symbol] = plugin;
                                        LogInfo(PluginConstants.Messages.PluginRegistered, plugin.AnimalName, plugin.Symbol);
                                    }
                                    else
                                    {
                                        LogWarning(PluginConstants.Messages.IncompatiblePlugin, pluginType.Name);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogError(ex, PluginConstants.Messages.PluginCreationError, pluginType.Name, ex.Message);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogError(ex, PluginConstants.Messages.PluginLoadError, Path.GetFileName(file), ex.Message);
                    }
                }

                LogInfo(PluginConstants.Messages.TotalPluginsLoaded, _loadedPlugins.Count);
            }
            catch (Exception ex)
            {
                LogError(ex, PluginConstants.Messages.PluginDirectoryError, ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Gets a plugin by its symbol character
        /// </summary>
        /// <param name="symbol">The symbol character of the plugin</param>
        /// <returns>The plugin if found, null otherwise</returns>
        public IAnimalPlugin? GetPlugin(char symbol)
        {
            return _loadedPlugins.TryGetValue(symbol, out var plugin) ? plugin : null;
        }

        /// <summary>
        /// Gets all loaded plugins
        /// </summary>
        /// <returns>Collection of all loaded plugins</returns>
        public IEnumerable<IAnimalPlugin> GetAllPlugins()
        {
            return _loadedPlugins.Values;
        }

        /// <summary>
        /// Gets the type for a specific animal from the loaded assemblies
        /// </summary>
        /// <param name="animalName">The name of the animal type to find</param>
        /// <returns>The type if found, null otherwise</returns>
        public Type? GetAnimalType(string animalName)
        {
            foreach (var assembly in _loadedAssemblies.Values)
            {
                try
                {
                    var type = assembly.GetTypes()
                        .FirstOrDefault(t => typeof(IGameEntity).IsAssignableFrom(t) && 
                                          t.Name.Equals(animalName, StringComparison.OrdinalIgnoreCase));
                    
                    if (type != null)
                    {
                        return type;
                    }
                }
                catch (Exception ex)
                {
                    LogError(ex, PluginConstants.Messages.TypeLoadError, assembly.FullName);
                }
            }
            return null;
        }

        private void LogInfo(string message, params object[] args)
        {
            if (_logger != null)
            {
                _logger.LogInformation(message, args);
            }
            else
            {
                Console.WriteLine(string.Format(message, args));
            }
        }

        private void LogWarning(string message, params object[] args)
        {
            if (_logger != null)
            {
                _logger.LogWarning(message, args);
            }
            else
            {
                Console.WriteLine($"WARNING: {string.Format(message, args)}");
            }
        }

        private void LogError(Exception ex, string message, params object[] args)
        {
            if (_logger != null)
            {
                _logger.LogError(ex, message, args);
            }
            else
            {
                Console.WriteLine($"ERROR: {string.Format(message, args)}");
                Console.WriteLine($"Exception: {ex}");
            }
        }
    }
} 