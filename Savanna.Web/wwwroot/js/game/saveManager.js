// Save Management
class SaveManager {
    async loadSavedGames() {
        try {
            const response = await fetch(GameConstants.Api.Endpoints.SAVED);
            if (!response.ok) throw new Error('Failed to load saved games');
            
            const savedGames = await response.json();
            const container = document.getElementById('savedGames');
            
            if (container) {
                container.innerHTML = ''; // Clear existing saves
                
                savedGames.forEach(game => {
                    const gameItem = document.createElement('div');
                    gameItem.className = 'saved-game-item';
                    
                    // Create header with title and date
                    const header = document.createElement('div');
                    header.className = 'saved-game-header';
                    
                    const title = document.createElement('h6');
                    title.className = 'saved-game-title';
                    title.textContent = `Game ${game.name}`;
                    
                    const date = document.createElement('small');
                    date.className = 'saved-game-date';
                    date.textContent = new Date(game.saveDate).toLocaleString();
                    
                    header.appendChild(title);
                    header.appendChild(date);
                    
                    // Create stats section
                    const stats = document.createElement('div');
                    stats.className = 'saved-game-stats';
                    stats.innerHTML = `
                        <span>Iteration: ${game.iteration}</span>
                        <span>•</span>
                        <span>Lions: ${game.animalCounts.lion || 0}</span>
                        <span>•</span>
                        <span>Antelopes: ${game.animalCounts.antelope || 0}</span>
                        <span>•</span>
                        <span>Tigers: ${game.animalCounts.tiger || 0}</span>
                        <span>•</span>
                        <span>Zebras: ${game.animalCounts.zebra || 0}</span>
                    `;
                    
                    // Create controls section
                    const controls = document.createElement('div');
                    controls.className = 'saved-game-controls';
                    controls.innerHTML = `
                        <button class="btn btn-outline-primary" onclick="saveManager.loadGame(${game.id}, ${game.iteration})">
                            <i class="bi bi-play-fill"></i> Load
                        </button>
                        <button class="btn btn-outline-danger" onclick="saveManager.deleteSave(${game.id})">
                            <i class="bi bi-trash"></i>
                        </button>
                    `;
                    
                    // Assemble all sections
                    gameItem.appendChild(header);
                    gameItem.appendChild(stats);
                    gameItem.appendChild(controls);
                    
                    container.appendChild(gameItem);
                });
            }
        } catch (error) {
            console.error('Error loading saved games:', error);
            uiManager.showErrorMessage('Failed to load saved games');
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