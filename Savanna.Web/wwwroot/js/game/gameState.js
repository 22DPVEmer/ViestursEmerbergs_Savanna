// Game state management
class GameState {
    constructor() {
        this.gameActive = false;
        this.hasUnsavedChanges = false;
        this.isPaused = false;
        this.displayMode = localStorage.getItem('displayMode') || 'icons';
        this.gameUpdateInterval = null;
        this.POLL_INTERVAL = 1000;
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
                throw new Error('Failed to get game state');
            }

            const gameState = await response.json();
            uiManager.updateUI(gameState);
            this.errorCount = 0;
        } catch (error) {
            console.error('Error updating game state:', error);
            this.handleUpdateError();
        }
    }

    handleUpdateError() {
        this.errorCount++;
        if (this.errorCount > 3) {
            this.stopGameStatePolling();
            uiManager.showErrorMessage('Lost connection to the game. Please refresh the page.');
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