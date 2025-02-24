using Savanna.Common.Plugin;

namespace Savanna.GameEngine.Plugin
{
    /// <summary>
    /// Alias for the unified PluginLoader from Common
    /// This maintains backward compatibility for the GameEngine
    /// </summary>
    public class PluginLoader : Common.Plugin.PluginLoader
    {
        public PluginLoader(string pluginPath) : base(pluginPath)
        {
        }
    }
}