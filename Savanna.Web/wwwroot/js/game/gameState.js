// Game state management
class GameState {
    constructor() {
        this.gameActive = false;
        this.hasUnsavedChanges = false;
        this.isPaused = false;
        this.displayMode = localStorage.getItem('displayMode') || 'icons';
        this.gameUpdateInterval = null;
        this.POLL_INTERVAL = GameConstants.Intervals.StatePolling;
        this.errorCount = 0;
    }

    async updateGameState() {
        try {
            const response = await fetch('/api/games/update', { method: 'POST' });
            if (!response.ok) {
                if (response.status === 401) {
                    window.location.href = '/Account/Login';
                    return;
                }
                if (response.status === 404) {
                    this.stopGameStatePolling();
                    return;
                }
                throw new Error(GameConstants.Messages.StateUpdateFailed);
            }

            const gameState = await response.json();
            uiManager.updateUI(gameState);
            this.errorCount = 0;
        } catch (error) {
            this.handleUpdateError();
        }
    }

    handleUpdateError() {
        this.errorCount++;
        if (this.errorCount > GameConstants.Intervals.MaxErrorRetries) {
            this.stopGameStatePolling();
            uiManager.showErrorMessage(GameConstants.Messages.LostConnection);
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