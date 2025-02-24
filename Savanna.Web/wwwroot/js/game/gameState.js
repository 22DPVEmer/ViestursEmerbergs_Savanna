// Game state management
class GameState {
    constructor() {
        this.gameActive = false;
        this.hasUnsavedChanges = false;
        this.isPaused = false;
        this.displayMode = localStorage.getItem(GameConstants.LocalStorage.Keys.DISPLAY_MODE) || GameConstants.Game.Display.ICONS;
        this.gameUpdateInterval = null;
        this.POLL_INTERVAL = GameConstants.Game.Intervals.STATE_POLLING;
        this.errorCount = 0;
    }

    async startGameStatePolling() {
        if (this.gameUpdateInterval) {
            clearInterval(this.gameUpdateInterval);
        }

        this.gameUpdateInterval = setInterval(async () => {
            try {
                const response = await fetch(GameConstants.Api.Endpoints.UPDATE, { 
                    method: GameConstants.Api.Methods.POST 
                });

                if (!response.ok) {
                    if (response.status === GameConstants.Api.StatusCodes.UNAUTHORIZED) {
                        this.stopGameStatePolling();
                        return;
                    }
                    throw new Error(GameConstants.Messages.Error.CONNECTION_LOST);
                }

                const state = await response.json();
                if (!state) {
                    throw new Error(GameConstants.Messages.Error.NO_ACTIVE_GAME);
                }

                uiManager.updateUI(state);
                this.errorCount = 0;
            } catch (error) {
                this.errorCount++;
                if (this.errorCount >= 3) {
                    this.stopGameStatePolling();
                    uiManager.showErrorMessage(GameConstants.Messages.Error.CONNECTION_LOST);
                }
            }
        }, this.POLL_INTERVAL);
    }

    stopGameStatePolling() {
        if (this.gameUpdateInterval) {
            clearInterval(this.gameUpdateInterval);
            this.gameUpdateInterval = null;
        }
    }

    resetGameState() {
        this.gameActive = false;
        this.hasUnsavedChanges = false;
        this.isPaused = false;
        this.stopGameStatePolling();
        uiManager.resetUI();
    }
}

// Export as global instance
window.gameState = new GameState(); 