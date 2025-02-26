// Game Controls
class GameControls {
    async startGame() {
        try {
            console.log(GameConstants.UI.Messages.Console.UPDATING_STATE);
            const response = await fetch(GameConstants.Api.Endpoints.START, { 
                method: GameConstants.Api.Methods.POST 
            });
            if (!response.ok) throw new Error(GameConstants.UI.Messages.Error.FAILED_TO_START);
            
            const data = await response.json();
            console.log(data.message);
            
            // Wait for game to initialize with retries
            let retries = 5;
            let stateData = null;
            let lastError = null;
            
            while (retries > 0 && !stateData) {
                try {
                    await new Promise(resolve => setTimeout(resolve, 1000));
                    console.log(GameConstants.UI.Messages.Console.VERIFYING_STATE(retries));
                    
                    const stateResponse = await fetch(GameConstants.Api.Endpoints.STATE);
                    console.log(GameConstants.UI.Messages.Console.RESPONSE_STATUS(stateResponse.status));
                    
                    if (stateResponse.ok) {
                        stateData = await stateResponse.json();
                        if (stateData) {
                            console.log(GameConstants.UI.Messages.Console.STATE_VERIFIED(stateData));
                            break;
                        }
                    } else {
                        lastError = GameConstants.UI.Messages.Error.STATE_VERIFICATION_FAILED(stateResponse.status);
                    }
                    
                    retries--;
                } catch (error) {
                    console.warn(GameConstants.UI.Messages.Console.STATE_VERIFY_FAILED(error));
                    lastError = error.message;
                    retries--;
                }
            }
            
            if (!stateData) {
                throw new Error(lastError || GameConstants.UI.Messages.Error.FAILED_TO_VERIFY_STATE);
            }
            
            gameState.gameActive = true;
            uiManager.enableGameControls();

            // Add initial animals without starting polling
            console.log('Adding initial animals...');
            await animalManager.addAnimal(GameConstants.Animals.Types.LION, true);
            await animalManager.addAnimal(GameConstants.Animals.Types.ANTELOPE, true);
            console.log('Initial animals added successfully');

            // Start polling after initial animals are added
            gameState.startGameStatePolling();
        } catch (error) {
            console.error('Error starting game:', error);
            uiManager.showErrorMessage(GameConstants.UI.Messages.Error.FAILED_TO_START);
            gameState.resetGameState();
        }
    }

    async quitGame() {
        try {
            const response = await fetch(GameConstants.Api.Endpoints.QUIT, { 
                method: GameConstants.Api.Methods.POST 
            });
            if (!response.ok) throw new Error(GameConstants.UI.Messages.Error.FAILED_TO_QUIT);
            
            const data = await response.json();
            console.log(data.message);
            
            gameState.gameActive = false;
            gameState.stopGameStatePolling();
            gameState.resetGameState();
        } catch (error) {
            console.error('Error quitting game:', error);
            uiManager.showErrorMessage(GameConstants.UI.Messages.Error.FAILED_TO_QUIT);
        }
    }

    async togglePause() {
        try {
            const response = await fetch(GameConstants.Api.Endpoints.TOGGLE_PAUSE, { 
                method: GameConstants.Api.Methods.POST 
            });
            if (!response.ok) throw new Error(GameConstants.UI.Messages.Error.FAILED_TO_TOGGLE_PAUSE);
            
            const data = await response.json();
            gameState.isPaused = data.isPaused;
            uiManager.updatePauseButtonText();
        } catch (error) {
            console.error('Error toggling pause state:', error);
            uiManager.showErrorMessage(GameConstants.UI.Messages.Error.FAILED_TO_TOGGLE_PAUSE);
        }
    }
}

// Export as global instance
window.gameControls = new GameControls(); 