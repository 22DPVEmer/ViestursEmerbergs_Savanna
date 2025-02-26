// Game state management
class GameState {
    constructor() {
        this.gameActive = false;
        this.hasUnsavedChanges = false;
        this.isPaused = false;
        this.displayMode = localStorage.getItem(GameConstants.Storage.Keys.DISPLAY_MODE) || GameConstants.Storage.DefaultValues.DISPLAY_MODE;
        this.gameUpdateInterval = null;
        this.POLL_INTERVAL = GameConstants.Game.Intervals.STATE_POLLING;
        this.errorCount = 0;
    }

    async updateGameState() {
        try {
            const response = await fetch(GameConstants.Api.Endpoints.UPDATE, { 
                method: GameConstants.Api.Methods.POST 
            });
            
            if (!response.ok) {
                if (response.status === GameConstants.Api.StatusCodes.UNAUTHORIZED) {
                    window.location.href = GameConstants.Api.Endpoints.LOGIN;
                    return;
                }
                if (response.status === GameConstants.Api.StatusCodes.NOT_FOUND) {
                    this.stopGameStatePolling();
                    return;
                }
                throw new Error(GameConstants.UI.Messages.Error.FAILED_TO_GET_STATE);
            }

            const gameState = await response.json();
            uiManager.updateUI(gameState);
            this.errorCount = 0;
        } catch (error) {
            console.error(GameConstants.UI.Messages.Console.ERROR_UPDATE(error));
            this.handleUpdateError();
        }
    }

    handleUpdateError() {
        this.errorCount++;
        if (this.errorCount > GameConstants.Game.Limits.MAX_ERROR_COUNT) {
            this.stopGameStatePolling();
            uiManager.showErrorMessage(GameConstants.UI.Messages.Error.CONNECTION_LOST);
        }
    }

    startGameStatePolling() {
        if (this.gameUpdateInterval) {
            clearInterval(this.gameUpdateInterval);
        }
        this.gameUpdateInterval = setInterval(() => this.updateGameState(), this.POLL_INTERVAL);
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