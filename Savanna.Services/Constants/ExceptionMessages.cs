namespace Savanna.Services.Constants
{
    public static class ExceptionMessages
    {
        public static class Configuration
        {
            public const string FileNotFound = "Configuration file not found";
            public const string DeserializationFailed = "Failed to deserialize configuration file";
            public const string PluginsSectionMissing = "Plugins section is missing in configuration";
            public const string NoPluginsFound = "No plugins found in configuration";
            public const string InvalidJsonFormat = "Invalid JSON format in configuration file";
            public const string LoadConfigurationFailed = "Failed to load animal configurations";
        }

        public static class Game
        {
            public const string GameNotFound = "Game instance not found";
            public const string GameAlreadyExists = "Game already exists for user";
            public const string InvalidGameState = "Invalid game state";
            public const string SaveNotFound = "Save game not found";
        }

        public static class Plugin
        {
            public const string PluginLoadFailed = "Failed to load plugin";
            public const string PluginNotFound = "Plugin not found";
            public const string InvalidPluginFormat = "Invalid plugin format";
        }

        public static class User
        {
            public const string UserNotFound = "User not found";
            public const string UnauthorizedAccess = "Unauthorized access to resource";
        }
    }
} 