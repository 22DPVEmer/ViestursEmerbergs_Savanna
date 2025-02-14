namespace Savanna.Common.Constants
{
    /// <summary>
    /// Contains all plugin-related constants and messages
    /// </summary>
    public static class PluginConstants
    {
        public const string DllSearchPattern = "*.dll";

        public static class Messages
        {
            // Plugin Loader messages
            public const string SearchingPlugins = "\nSearching for plugins in: {0}";
            public const string FoundDllFiles = "Found {0} DLL files";
            public const string LoadingPluginFile = "\nLoading plugin file: {0}";
            public const string FoundPluginType = "Found plugin type: {0}";
            public const string LoadedPlugin = "Loaded plugin: {0} with symbol '{1}'";
            public const string LoadingError = "Error loading plugin {0}: {1}";
            public const string InnerException = "Inner exception: {0}";
            public const string TotalPluginsLoaded = "\nTotal plugins loaded: {0}";

            // Plugin Manager messages
            public const string PluginRegistered = "Loaded plugin: {0} (Symbol: {1})";
            public const string IncompatiblePlugin = "Incompatible plugin: {0}";
            public const string PluginNotFound = "No plugin found for animal symbol: {0}";

            // Console App Plugin messages
            public const string PluginDirectory = "Plugin directory: {0}";
            public const string PluginDirectoryError = "Warning: Failed to create plugins directory: {0}";
            public const string AvailableAnimalsHeader = "\nAvailable animals:";
            public const string AnimalListItem = "- {0}";
        }
    }
}