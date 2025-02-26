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
            ANIMAL_TYPES: '/api/games/animal-types',
            LOGIN: '/Account/Login',
            DELETE_SAVE: (id) => `/api/games/saved/${id}`,
            LOAD_SAVE: (id) => `/api/games/load/${id}`
        },
        Methods: {
            GET: 'GET',
            POST: 'POST',
            DELETE: 'DELETE'
        },
        Headers: {
            CONTENT_TYPE: 'Content-Type',
            JSON: 'application/json'
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
            TOTAL_CELLS: 400,
            TEMPLATE_COLUMNS: 'repeat(20, 1fr)'
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
        },
        Limits: {
            MAX_ERROR_COUNT: 3
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
        },
        HexColors: {
            LION: '#dc3545',
            ANTELOPE: '#198754',
            TIGER: '#fd7e14',
            ZEBRA: '#6c757d'
        }
    },

    UI: {
        Elements: {
            IDs: {
                GAME_GRID: 'gameGrid',
                ANIMAL_CONTROLS: 'animalControls',
                ANIMAL_STATS: 'animalStats',
                START_GAME: 'startGame',
                QUIT_GAME: 'quitGame',
                SAVE_GAME: 'saveGame',
                PAUSE_GAME: 'pauseGame',
                ITERATION_COUNTER: 'iterationCounter',
                ICON_MODE: 'iconMode',
                TEXT_MODE: 'textMode',
                SAVED_GAMES: 'savedGames'
            },
            Classes: {
                CARD_BODY: '.card-body',
                GAME_CELL: 'game-cell',
                LIST_GROUP_ITEM: 'list-group-item d-flex justify-content-between align-items-center',
                BADGE: 'badge bg-{0} rounded-pill',
                BUTTON: 'btn btn-outline-{0}',
                BUTTON_ADD_PREFIX: '[id^="add"]',
                ALERT_ERROR: 'alert alert-danger alert-dismissible fade show',
                ALERT_SUCCESS: 'alert alert-success alert-dismissible fade show',
                SAVED_GAME_ITEM: 'list-group-item',
                NO_SAVES_MESSAGE: 'list-group-item text-center text-muted',
                SAVED_GAME_HEADER: 'mb-1',
                SAVED_GAME_CONTROLS: 'btn-group btn-group-sm',
                SAVED_GAME_STATS: 'mt-2'
            },
            DataAttributes: {
                X: 'data-x',
                Y: 'data-y',
                ANIMAL_TYPE: 'data-animal-type'
            },
            Icons: {
                ADD: '<i class="bi bi-plus-circle"></i>',
                PLAY: '<i class="bi bi-play-fill"></i>',
                TRASH: '<i class="bi bi-trash"></i>'
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
                CONNECTION_LOST: 'Lost connection to the game. Please refresh the page.',
                NO_SAVED_GAMES: 'No saved games found',
                GAME_NOT_ACTIVE: 'Cannot add animals - game is not active',
                FAILED_TO_GET_STATE: 'Failed to get game state',
                FAILED_TO_INIT_CONTROLS: 'Failed to initialize animal controls. Please refresh the page.',
                FAILED_TO_INIT_STATS: 'Error initializing animal statistics',
                GENERIC_ERROR: (action, error) => `Error ${action}: ${error}`,
                FAILED_TO_LOAD_SAVES: 'Failed to load saved games',
                FAILED_TO_VERIFY_STATE: 'Could not verify loaded game state',
                FAILED_TO_DELETE_SAVE: 'Failed to delete save',
                STATE_VERIFICATION_FAILED: (status) => `State verification failed with status ${status}`,
                SAVE_LOAD_ERROR: (error) => `Failed to load game: ${error}`,
                SAVE_DELETE_ERROR: (error) => `Failed to delete save: ${error}`
            },
            Console: {
                ADDING_ANIMAL: (type) => `Adding animal of type: ${type}`,
                ANIMAL_ADDED: (message) => `Animal added successfully: ${message}`,
                UPDATING_STATE: 'Updating game state...',
                ERROR_ADDING: (type, error) => `Error adding animal: ${type}, ${error}`,
                ERROR_UPDATE: (error) => `Error updating game state: ${error}`,
                LOADING_SAVES: 'Loading saved games...',
                RESPONSE_STATUS: (status) => `Response status: ${status}`,
                NOT_AUTHENTICATED: 'Not authenticated, skipping saves display',
                CONTAINER_NOT_FOUND: 'Saved games container not found',
                LOADED_SAVES: (saves) => `Loaded saved games: ${saves}`,
                PROCESSING_GAME: (game) => `Processing game: ${game}`,
                LOADING_GAME: (id, iteration) => `Loading game: ${id} with iteration: ${iteration}`,
                VERIFYING_STATE: (retries) => `Verifying loaded game state (${retries} attempts left)...`,
                STATE_VERIFIED: (data) => `Loaded game state verified: ${data}`,
                STATE_VERIFY_FAILED: (error) => `State verification attempt failed: ${error}`,
                SAVE_RESPONSE: (data) => `Save response: ${data}`
            },
            Confirm: {
                DELETE_SAVE: 'Are you sure you want to delete this saved game?'
            },
            Labels: {
                LOAD: 'Load',
                SAVED_DATE: (date) => `Saved: ${new Date(date).toLocaleString()}`,
                ITERATION: (iteration) => `Iteration: ${iteration}`,
                LIONS: (count) => `Lions: ${count}`,
                ANTELOPES: (count) => `Antelopes: ${count}`,
                TIGERS: (count) => `Tigers: ${count}`,
                ZEBRAS: (count) => `Zebras: ${count}`,
                NO_SAVES: 'No saved games found'
            },
            Buttons: {
                RESUME_GAME: 'Resume Game',
                PAUSE_GAME: 'Pause Game',
                ADD_ANIMAL: (type) => `Add ${type}`
            }
        }
    },

    Storage: {
        Keys: {
            DISPLAY_MODE: 'displayMode',
            USER_SETTINGS: 'userSettings',
            GAME_STATE: 'gameState',
            ANONYMOUS_USER_ID: 'AnonymousUserId'
        },
        DefaultValues: {
            DISPLAY_MODE: 'icons'
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
    },

    AnimalTypes: {
        LION: 'Lion',
        ANTELOPE: 'Antelope',
        TIGER: 'Tiger',
        ZEBRA: 'Zebra'
    },

    Icons: {
        Lion: '🦁',
        Antelope: '🦌',
        Tiger: '🐯',
        Zebra: '🦓'
    },

    TextSymbols: {
        Lion: 'L',
        Antelope: 'A',
        Tiger: 'T',
        Zebra: 'Z'
    },

    Colors: {
        Lion: 'danger',
        Antelope: 'success',
        Tiger: 'warning',
        Zebra: 'secondary'
    },

    CSS: {
        ALERT_ERROR: 'alert alert-danger alert-dismissible fade show',
        ALERT_SUCCESS: 'alert alert-success alert-dismissible fade show',
        GAME_CELL: 'game-cell',
        BUTTON_ADD_PREFIX: '[id^="add"]'
    },

    ElementIds: {
        START_GAME: 'startGame',
        QUIT_GAME: 'quitGame',
        SAVE_GAME: 'saveGame',
        PAUSE_GAME: 'pauseGame',
        ITERATION_COUNTER: 'iterationCounter'
    },

    Messages: {
        Errors: {
            GAME_NOT_ACTIVE: 'Cannot add animals - game is not active',
            LOST_CONNECTION: 'Lost connection to the game. Please refresh the page.',
            ADD_ANIMAL_FAILED: (type) => `Failed to add ${type}`,
            NO_ACTIVE_GAME: 'No active game'
        },
        Buttons: {
            RESUME_GAME: 'Resume Game',
            PAUSE_GAME: 'Pause Game'
        },
        Console: {
            ADDING_ANIMAL: (type) => `Adding animal of type: ${type}`,
            ANIMAL_ADDED: (message) => `Animal added successfully: ${message}`,
            UPDATING_STATE: 'Updating game state...',
            ERROR_ADDING: (type, error) => `Error adding animal: ${type}, ${error}`
        }
    },

    GameDefaults: {
        GRID_SIZE: 20,
        ALERT_TIMEOUT: 5000,
        POLL_INTERVAL: 1000,
        DISPLAY_MODE_KEY: 'displayMode',
        DEFAULT_DISPLAY_MODE: 'icons'
    },

    LocalStorage: {
        DISPLAY_MODE: 'displayMode'
    },

    ApiEndpoints: {
        ADD_ANIMAL: '/api/games/add-animal',
        GAME_STATE: '/api/games/state',
        UPDATE_GAME: '/api/games/update',
        LOGIN: '/Account/Login'
    }
}; 