// Game Constants
const GameConstants = {
    Messages: {
        // Success messages
        GameStarted: 'Game started successfully',
        GameQuit: 'Game quit successfully',
        AnimalAdded: 'Animal added successfully',
        GameSaved: 'Game saved successfully',
        SaveDeleted: 'Game save deleted successfully',
        GameLoaded: 'Game loaded successfully',
        
        // Error messages
        NoActiveGame: 'Cannot add animals - game is not active',
        FailedToStart: 'Failed to start game',
        FailedToQuit: 'Failed to quit game. Please try again.',
        FailedToAddAnimal: 'Failed to add animal',
        FailedToTogglePause: 'Failed to toggle pause state. Please try again.',
        FailedToInitControls: 'Failed to initialize animal controls. Please refresh the page.',
        LostConnection: 'Lost connection to the game. Please refresh the page.',
        FailedToLoadSaves: 'Failed to load saved games',
        FailedToLoadGame: 'Failed to load game',
        FailedToSaveGame: 'Failed to save game',
        FailedToDeleteSave: 'Failed to delete save',
        NoSavedGames: 'No saved games found',
        
        // Debug messages
        StateUpdateFailed: 'Failed to get game state',
        InitializationError: 'An error occurred during initialization'
    },

    Intervals: {
        StatePolling: 1000,  // 1 second
        MessageDisplay: 5000, // 5 seconds
        MaxErrorRetries: 3
    },

    Confirmations: {
        DeleteSave: 'Are you sure you want to delete this saved game?'
    }
}; 