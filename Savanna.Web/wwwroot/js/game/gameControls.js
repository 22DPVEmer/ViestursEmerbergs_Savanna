// Game Controls
class GameControls {
    async startGame() {
        try {
            const response = await fetch(GameConstants.Api.Endpoints.START, { 
                method: GameConstants.Api.Methods.POST 
            });
            
            if (!response.ok) {
                throw new Error(GameConstants.Messages.Error.FAILED_TO_START);
            }
            
            const data = await response.json();
            gameState.gameActive = true;
            uiManager.enableGameControls();

            // Add initial animals
            await animalManager.addAnimal(GameConstants.Animals.Types.LION, true);
            await animalManager.addAnimal(GameConstants.Animals.Types.ANTELOPE, true);

            // Start polling
            gameState.startGameStatePolling();
            uiManager.showSuccessMessage(GameConstants.Messages.Success.GAME_STARTED);
        } catch (error) {
            uiManager.showErrorMessage(error.message);
            gameState.resetGameState();
        }
    }

    async quitGame() {
        if (!confirm(GameConstants.Messages.Confirm.QUIT_GAME)) {
            return;
        }

        try {
            const response = await fetch(GameConstants.Api.Endpoints.QUIT, { 
                method: GameConstants.Api.Methods.POST 
            });
            
            if (!response.ok) {
                throw new Error(GameConstants.Messages.Error.FAILED_TO_QUIT);
            }
            
            gameState.gameActive = false;
            gameState.stopGameStatePolling();
            gameState.resetGameState();
            uiManager.showSuccessMessage(GameConstants.Messages.Success.GAME_QUIT);
        } catch (error) {
            uiManager.showErrorMessage(GameConstants.Messages.Error.FAILED_TO_QUIT);
        }
    }

    async togglePause() {
        try {
            const response = await fetch(GameConstants.Api.Endpoints.TOGGLE_PAUSE, { 
                method: GameConstants.Api.Methods.POST 
            });
            
            if (!response.ok) {
                throw new Error(GameConstants.Messages.Error.FAILED_TO_TOGGLE_PAUSE);
            }
            
            const data = await response.json();
            gameState.isPaused = data.isPaused;
            uiManager.updatePauseButtonText();
        } catch (error) {
            uiManager.showErrorMessage(GameConstants.Messages.Error.FAILED_TO_TOGGLE_PAUSE);
        }
    }
}

// Export as global instance
window.gameControls = new GameControls(); 