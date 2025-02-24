// Game Controls
class GameControls {
    async startGame() {
        try {
            const response = await fetch('/api/games/start', { method: 'POST' });
            if (!response.ok) throw new Error(GameConstants.Messages.FailedToStart);
            
            const data = await response.json();
            gameState.gameActive = true;
            uiManager.enableGameControls();

            // Add initial animals
            await animalManager.addAnimal('Lion', true);
            await animalManager.addAnimal('Antelope', true);

            // Start polling
            gameState.startGameStatePolling();
        } catch (error) {
            uiManager.showErrorMessage(error.message);
            gameState.resetGameState();
        }
    }

    async quitGame() {
        try {
            const response = await fetch('/api/games/quit', { method: 'POST' });
            if (!response.ok) throw new Error(GameConstants.Messages.FailedToQuit);
            
            gameState.gameActive = false;
            gameState.stopGameStatePolling();
            gameState.resetGameState();
        } catch (error) {
            uiManager.showErrorMessage(GameConstants.Messages.FailedToQuit);
        }
    }

    async togglePause() {
        try {
            const response = await fetch('/api/games/toggle-pause', { method: 'POST' });
            if (!response.ok) throw new Error(GameConstants.Messages.FailedToTogglePause);
            
            const data = await response.json();
            gameState.isPaused = data.isPaused;
            uiManager.updatePauseButtonText();
        } catch (error) {
            uiManager.showErrorMessage(GameConstants.Messages.FailedToTogglePause);
        }
    }
}

// Export as global instance
window.gameControls = new GameControls(); 