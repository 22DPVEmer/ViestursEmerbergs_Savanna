// Save Management
class SaveManager {
    async loadSavedGames() {
        try {
            console.log(GameConstants.UI.Messages.Console.LOADING_SAVES);
            const response = await fetch(GameConstants.Api.Endpoints.SAVED);
            console.log(GameConstants.UI.Messages.Console.RESPONSE_STATUS(response.status));
            
            if (!response.ok) {
                if (response.status === GameConstants.Api.StatusCodes.UNAUTHORIZED) {
                    console.log(GameConstants.UI.Messages.Console.NOT_AUTHENTICATED);
                    return;
                }
                throw new Error(GameConstants.UI.Messages.Error.FAILED_TO_LOAD_SAVES);
            }
            
            const savedGames = await response.json();
            console.log(GameConstants.UI.Messages.Console.LOADED_SAVES(savedGames));
            
            const container = document.getElementById(GameConstants.UI.Elements.IDs.SAVED_GAMES);
            if (!container) {
                console.error(GameConstants.UI.Messages.Console.CONTAINER_NOT_FOUND);
                return;
            }
            
            container.innerHTML = ''; // Clear existing games
            
            if (savedGames.length === 0) {
                container.innerHTML = `
                    <div class="${GameConstants.UI.Elements.Classes.NO_SAVES_MESSAGE}">
                        ${GameConstants.UI.Messages.Labels.NO_SAVES}
                    </div>
                `;
                return;
            }
            
            savedGames.forEach(game => {
                console.log(GameConstants.UI.Messages.Console.PROCESSING_GAME(game));
                const lionCount = game.animalCounts.lion || 0;
                const antelopeCount = game.animalCounts.antelope || 0;
                const tigerCount = game.animalCounts.tiger || 0;
                const zebraCount = game.animalCounts.zebra || 0;
                
                const gameItem = document.createElement('div');
                gameItem.className = GameConstants.UI.Elements.Classes.SAVED_GAME_ITEM;
                gameItem.innerHTML = `
                    <div class="d-flex w-100 justify-content-between align-items-center">
                        <div>
                            <h6 class="${GameConstants.UI.Elements.Classes.SAVED_GAME_HEADER}">${game.name}</h6>
                            <small class="text-muted">${GameConstants.UI.Messages.Labels.SAVED_DATE(game.saveDate)}</small>
                        </div>
                        <div class="${GameConstants.UI.Elements.Classes.SAVED_GAME_CONTROLS}">
                            <button class="btn btn-outline-primary" onclick="saveManager.loadGame(${game.id}, ${game.iteration})">
                                ${GameConstants.UI.Elements.Icons.PLAY} ${GameConstants.UI.Messages.Labels.LOAD}
                            </button>
                            <button class="btn btn-outline-danger" onclick="saveManager.deleteSave(${game.id})">
                                ${GameConstants.UI.Elements.Icons.TRASH}
                            </button>
                        </div>
                    </div>
                    <div class="${GameConstants.UI.Elements.Classes.SAVED_GAME_STATS}">
                        <small class="me-2">${GameConstants.UI.Messages.Labels.ITERATION(game.iteration)}</small>
                        <small class="me-2">${GameConstants.UI.Messages.Labels.LIONS(lionCount)}</small>
                        <small class="me-2">${GameConstants.UI.Messages.Labels.ANTELOPES(antelopeCount)}</small>
                        <small class="me-2">${GameConstants.UI.Messages.Labels.TIGERS(tigerCount)}</small>
                        <small>${GameConstants.UI.Messages.Labels.ZEBRAS(zebraCount)}</small>
                    </div>
                `;
                container.appendChild(gameItem);
            });
        } catch (error) {
            console.error('Error loading saved games:', error);
            uiManager.showErrorMessage(GameConstants.UI.Messages.Error.FAILED_TO_LOAD_SAVES);
        }
    }

    async loadGame(saveId, savedIteration) {
        try {
            console.log(GameConstants.UI.Messages.Console.LOADING_GAME(saveId, savedIteration));
            
            // First, quit any existing game
            if (gameState.gameActive) {
                await gameControls.quitGame();
            }
            
            const response = await fetch(GameConstants.Api.Endpoints.LOAD_SAVE(saveId), {
                method: GameConstants.Api.Methods.POST
            });
            
            if (!response.ok) {
                const data = await response.json();
                throw new Error(data.message || GameConstants.UI.Messages.Error.FAILED_TO_VERIFY_STATE);
            }
            
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
            gameState.startGameStatePolling();
            uiManager.updateUI(stateData);
            uiManager.showSuccessMessage(GameConstants.UI.Messages.Success.GAME_LOADED);
        } catch (error) {
            console.error('Error loading game:', error);
            uiManager.showErrorMessage(GameConstants.UI.Messages.Error.SAVE_LOAD_ERROR(error.message));
            gameState.resetGameState();
        }
    }

    async saveGame() {
        try {
            const response = await fetch(GameConstants.Api.Endpoints.SAVE, { 
                method: GameConstants.Api.Methods.POST 
            });
            
            if (!response.ok) {
                const data = await response.json();
                throw new Error(data.message || GameConstants.UI.Messages.Error.FAILED_TO_SAVE);
            }
            
            const data = await response.json();
            console.log(GameConstants.UI.Messages.Console.SAVE_RESPONSE(data));
            
            gameState.hasUnsavedChanges = false;
            await this.loadSavedGames(); // Refresh the saves list
            uiManager.showSuccessMessage(GameConstants.UI.Messages.Success.GAME_SAVED);
        } catch (error) {
            console.error('Error saving game:', error);
            uiManager.showErrorMessage(GameConstants.UI.Messages.Error.FAILED_TO_SAVE);
        }
    }

    async deleteSave(saveId) {
        if (!confirm(GameConstants.UI.Messages.Confirm.DELETE_SAVE)) {
            return;
        }
        
        try {
            const response = await fetch(GameConstants.Api.Endpoints.DELETE_SAVE(saveId), { 
                method: GameConstants.Api.Methods.DELETE 
            });
            
            if (!response.ok) {
                const data = await response.json();
                throw new Error(data.message || GameConstants.UI.Messages.Error.FAILED_TO_DELETE_SAVE);
            }
            
            await this.loadSavedGames(); // Refresh the list
            uiManager.showSuccessMessage(GameConstants.UI.Messages.Success.SAVE_DELETED);
        } catch (error) {
            console.error('Error deleting save:', error);
            uiManager.showErrorMessage(GameConstants.UI.Messages.Error.SAVE_DELETE_ERROR(error.message));
        }
    }
}

// Export as global instance
window.saveManager = new SaveManager(); 