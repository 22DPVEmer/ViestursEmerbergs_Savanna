namespace Savanna.Common.Constants
{
    /// <summary>
    /// Contains all plugin-related constants and messages
    /// </summary>
    public static class PluginConstants
    {
        public static class Paths
        {
            public const string PluginDirectory = "Plugins";
            public const string DllSearchPattern = "*.dll";
        }

        public static class Messages
        {
            // Plugin Loading messages
            public const string SearchingPlugins = "Searching for plugins in: {0}";
            public const string FoundDllFiles = "Found {0} DLL files";
            public const string LoadingPluginFile = "Loading plugin file: {0}";
            public const string FoundPluginType = "Found plugin type: {0}";
            public const string PluginRegistered = "Loaded plugin: {0} (Symbol: {1})";
            public const string IncompatiblePlugin = "Incompatible plugin: {0}";
            public const string PluginNotFound = "No plugin found for animal symbol: {0}";
            public const string TotalPluginsLoaded = "Total plugins loaded: {0}";

            // Error messages
            public const string PluginCreationError = "Error creating plugin instance for {0}: {1}";
            public const string PluginLoadError = "Error loading plugin file {0}: {1}";
            public const string PluginDirectoryError = "Error loading plugins from directory: {0}";
            public const string TypeLoadError = "Error getting types from assembly: {0}";

            // Console App Plugin messages
            public const string PluginDirectory = "Plugin directory: {0}";
            public const string AvailableAnimalsHeader = "\nAvailable animals:";
            public const string AnimalListItem = "- {0}";
        }
    }
}