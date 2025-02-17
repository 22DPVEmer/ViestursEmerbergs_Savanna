namespace Savanna.Common.Constants
{
    /// <summary>
    /// Contains message templates for plugin configuration-related operations
    /// </summary>
    public static class PluginConfigurationMessages
    {
        public const string ConfigFileNotFound = "{0} plugin configuration file not found. Tried:\n{1}\n{2}";
        public const string DeserializationFailed = "Failed to deserialize {0} configuration";
        public const string InitializationFailed = "Failed to initialize {0} plugin: {1}";
        public const string PluginNameMismatch = "Plugin name in config file ({0}) does not match expected name ({1})";
        public const string NotInitialized = "Plugin configuration not initialized. Call Initialize() first.";
        public const string CombinedConfigName = "Combined";

        public const string PluginsDirectory = "Plugins";
        public const string ConfigFileName = "config.json";
    }
} 