namespace Savanna.Web.Constants
{
    public static class LoggerMessages
    {
        public const string RetrievingSavedGames = "Retrieving saved games for user {UserId}";
        public const string RetrievedSavedGames = "Retrieved {Count} saved games for user {UserId}";
        public const string ErrorRetrievingSavedGames = "Error retrieving saved games for user {UserId}";
        public const string ErrorStartingGame = "Error starting game";
        public const string ErrorQuittingGame = "Error quitting game";
        public const string ErrorRetrievingGameState = "Error retrieving game state";
        public const string ErrorSavingGame = "Error saving game";
        public const string ErrorLoadingGame = "Error loading game";
        public const string AddAnimalEndpointCalled = "AddAnimal endpoint called with type: {Type}";
        public const string InvalidAnimalType = "Invalid animal type: null or empty";
        public const string AddingAnimal = "Adding animal of type {Type} for user {UserId}";
        public const string SuccessfullyAddedAnimal = "Successfully added animal of type {Type} for user {UserId}";
        public const string OperationErrorAddingAnimal = "Operation error while adding animal: {Message}";
        public const string InvalidArgumentAddingAnimal = "Invalid argument while adding animal: {Message}";
        public const string UnexpectedErrorAddingAnimal = "Unexpected error adding animal of type {Type}";
        public const string ErrorRetrievingAnimalDetails = "Error retrieving animal details";
        public const string ErrorUpdatingGameState = "Error updating game state";
        public const string ErrorTogglingGamePause = "Error toggling game pause state";
        public const string ErrorRetrievingAnimalTypes = "Error retrieving animal types";
        public const string ErrorDeletingSave = "Error deleting save {SaveId}";
    }

    public static class ResponseMessages
    {
        public const string GameStartedSuccess = "Game started successfully.";
        public const string GameQuitSuccess = "Game quit successfully.";
        public const string NoActiveGameFound = "No active game found";
        public const string GameSavedSuccess = "Game saved successfully";
        public const string GameLoadedSuccess = "Game loaded successfully";
        public const string AnimalTypeNullOrEmpty = "Animal type cannot be null or empty";
        public const string AnimalNotFound = "Animal not found";
        public const string SaveNotFound = "Save not found";
        public const string SaveDeletedSuccess = "Save deleted successfully";
        public const string ErrorRetrievingGameState = "Error retrieving game state";
        public const string ErrorRetrievingAnimalDetails = "Error retrieving animal details";
        public const string ErrorUpdatingGameState = "Error updating game state";
        public const string ErrorTogglingGamePause = "Error toggling game pause state";
        public const string ErrorRetrievingAnimalTypes = "Error retrieving animal types";
        public const string ErrorDeletingSave = "Error deleting save";
        public const string ErrorRetrievingSavedGames = "Error retrieving saved games: ";
        public const string ErrorStartingGame = "Error starting game";
        public const string ErrorQuittingGame = "Error quitting game";
        public const string ErrorSavingGame = "Error saving game";
        public const string ErrorLoadingGame = "Error loading game";
    }
} 