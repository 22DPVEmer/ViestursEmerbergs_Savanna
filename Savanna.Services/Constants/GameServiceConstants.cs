namespace Savanna.Services.Constants
{
    /// <summary>
    /// Contains all constants used in the GameService and related services
    /// </summary>
    public static class GameServiceConstants
    {
        public static class LogMessages
        {
            public const string ServiceInitialized = "GameService initialized. Games count: {Count}";
            public const string GettingGameId = "Getting or creating game ID for user {UserId}";
            public const string ExistingGameFound = "Found existing game ID {GameId} for user {UserId} after lock";
            public const string NewGameCreated = "Created new game with ID {GameId} for user {UserId}";
            public const string FailedToAddGame = "Failed to add new game with ID {GameId} to games dictionary";
            public const string NoGameIdFound = "No game ID found for user {UserId}";
            public const string GameInstanceNotFound = "Game instance not found for ID {GameId}";
            public const string StartingNewGame = "Starting new game for user {UserId}";
            public const string GameStarted = "Successfully started new game for user {UserId} with ID {GameId}";
            public const string FailedToStartGame = "Failed to start game for user {UserId}";
            public const string GameQuit = "Game quit for user {UserId}";
            public const string AddingAnimal = "Adding animal of type {Type} for user {UserId}";
            public const string NoActiveGame = "No active game found";
            public const string ConfigNotFound = "Configuration for animal type {Type} not found";
            public const string UsingSymbol = "Using symbol {Symbol} for animal type {Type}";
            public const string AnimalAdded = "Successfully added animal {Type} with symbol {Symbol} for user {UserId}";
            public const string FailedToAddAnimal = "Failed to add animal {Type} for user {UserId}";
            public const string GettingGameState = "Getting game state for user {UserId}. Current games count: {Count}";
            public const string UserMappings = "Current user mappings: {Mappings}";
            public const string NoGameIdInMapping = "No game ID found in user mapping for {UserId}";
            public const string GameNotFoundInDict = "Game {GameId} not found in games dictionary for user {UserId}";
            public const string GameStateRetrieved = "Retrieved game state for user {UserId}: Iteration {Iteration}, Animals {Count}";
            public const string SavingGame = "Starting save game process for user {UserEmail}";
            public const string UserNotFound = "User {UserEmail} not found in database";
            public const string CreatingGameState = "Creating game state entity with {Count} animals for user ID {UserId}";
            public const string AddingToDatabase = "Adding game state and save to database for user ID {UserId}";
            public const string GameSaved = "Successfully saved game with {Changes} changes for user {UserEmail} (ID: {UserId})";
            public const string DatabaseError = "Database error while saving game: {Message}";
            public const string ErrorSavingGame = "Error saving game for user {UserEmail}: {Message}";
            public const string LoadingGame = "Starting load game process for user {UserId}, save {SaveId}";
            public const string LoadingAnimal = "Loading {Type} at position ({X}, {Y})";
            public const string FailedToInitialize = "Failed to initialize loaded game";
            public const string AnimalNotFound = "Animal not found: {AnimalId}";
            public const string AnimalStateNotFound = "Animal state not found: {AnimalId}";
            public const string AttemptToPause = "Attempt to pause non-existent game for user {UserId}";
            public const string GamePauseState = "Game {State} for user {UserId}";
            public const string UpdatingGameState = "Starting update game state process for user {UserId}";
            public const string AttemptToUpdateNonExistent = "Attempt to update non-existent game for user {UserId}";
        }

        public static class ErrorMessages
        {
            public const string FailedToCreateGame = "Failed to create new game instance";
            public const string UserAccountNotFound = "User account not found. Please ensure you are logged in.";
            public const string FailedToGetGameState = "Failed to get game state";
            public const string FailedToSaveGame = "Failed to save game to database.";
        }

        public static class GameDefaults
        {
            public const string GameSaveNameFormat = "Game {0:yyyy-MM-dd HH:mm:ss}";
            public const int InvalidPosition = -1;
        }

        public static class PluginLoader
        {
            public static class Paths
            {
                public const string PluginDirectory = "Plugins";
                public const string PluginSearchPattern = "*.dll";
            }

            public static class LogMessages
            {
                public const string PluginLoaded = "Loaded plugin: {Plugin}";
                public const string PluginLoadFailed = "Failed to load plugin: {Plugin}";
                public const string PluginDirectoryLoadFailed = "Failed to load plugins from directory";
                public const string AnimalTypeError = "Error getting animal type from assembly: {Assembly}";
            }
        }
    }
} 