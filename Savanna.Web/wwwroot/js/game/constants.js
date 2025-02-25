// Game Constants
const GameConstants = {
    Api: {
        Endpoints: {
            UPDATE: '/api/games/update',
            START: '/api/games/start',
            QUIT: '/api/games/quit',
            SAVE: '/api/games/save',
            SAVED: '/api/games/saved',
            STATE: '/api/games/state',
            LOAD: '/api/games/load',
            ADD_ANIMAL: '/api/games/add-animal',
            TOGGLE_PAUSE: '/api/games/toggle-pause',
            ANIMAL_TYPES: '/api/games/animal-types'
        },
        Methods: {
            GET: 'GET',
            POST: 'POST',
            DELETE: 'DELETE'
        },
        StatusCodes: {
            OK: 200,
            BAD_REQUEST: 400,
            UNAUTHORIZED: 401,
            NOT_FOUND: 404,
            SERVER_ERROR: 500
        }
    },

    Game: {
        Grid: {
            WIDTH: 20,
            HEIGHT: 20,
            TOTAL_CELLS: 400
        },
        Display: {
            ICONS: 'icons',
            TEXT: 'text'
        },
        Storage: {
            DISPLAY_MODE: 'displayMode'
        },
        Intervals: {
            STATE_POLLING: 1000,
            MESSAGE_DISPLAY: 5000
        }
    },

    Animals: {
        Types: {
            LION: 'Lion',
            ANTELOPE: 'Antelope',
            TIGER: 'Tiger',
            ZEBRA: 'Zebra'
        },
        Icons: {
            LION: '🦁',
            ANTELOPE: '🦌',
            TIGER: '🐯',
            ZEBRA: '🦓'
        },
        Text: {
            LION: 'L',
            ANTELOPE: 'A',
            TIGER: 'T',
            ZEBRA: 'Z'
        },
        Colors: {
            LION: 'danger',
            ANTELOPE: 'success',
            TIGER: 'warning',
            ZEBRA: 'secondary'
        }
    },

    Messages: {
        Success: {
            GAME_STARTED: 'Game started successfully',
            GAME_SAVED: 'Game saved successfully',
            GAME_LOADED: 'Game loaded successfully',
            GAME_QUIT: 'Game quit successfully',
            SAVE_DELETED: 'Save deleted successfully',
            ANIMAL_ADDED: 'Animal added successfully'
        },
        Error: {
            NO_ACTIVE_GAME: 'No active game found',
            FAILED_TO_START: 'Failed to start game',
            FAILED_TO_QUIT: 'Failed to quit game',
            FAILED_TO_SAVE: 'Failed to save game',
            FAILED_TO_LOAD: 'Failed to load game',
            FAILED_TO_DELETE: 'Failed to delete save',
            FAILED_TO_ADD_ANIMAL: 'Failed to add animal',
            CONNECTION_LOST: 'Lost connection to the game',
            NO_SAVED_GAMES: 'No saved games found'
        },
        Confirm: {
            DELETE_SAVE: 'Are you sure you want to delete this saved game?',
            QUIT_GAME: 'Are you sure you want to quit? Unsaved progress will be lost.',
            UNSAVED_CHANGES: 'You have unsaved changes. Are you sure you want to leave?'
        }
    },

    LocalStorage: {
        Keys: {
            DISPLAY_MODE: 'displayMode',
            USER_SETTINGS: 'userSettings',
            GAME_STATE: 'gameState'
        }
    },

    SessionStorage: {
        Keys: {
            ANONYMOUS_USER_ID: 'AnonymousUserId'
        }
    },

    CSS: {
        Classes: {
            GAME_CELL: 'game-cell',
            LION: 'lion',
            ANTELOPE: 'antelope',
            TIGER: 'tiger',
            ZEBRA: 'zebra'
        }
    }
}; 