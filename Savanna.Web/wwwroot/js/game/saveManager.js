// Save Management
class SaveManager {
    async loadSavedGames() {
        try {
            const response = await fetch('/api/games/saved');
            
            if (!response.ok) {
                if (response.status === 401) {
                    return; // Not logged in, just don't show saves
                }
                throw new Error(GameConstants.Messages.FailedToLoadSaves);
            }
            
            const savedGames = await response.json();
            const container = document.getElementById('savedGames');
            if (!container) return;
            
            container.innerHTML = ''; // Clear existing games
            
            if (savedGames.length === 0) {
                container.innerHTML = `
                    <div class="list-group-item text-center text-muted">
                        ${GameConstants.Messages.NoSavedGames}
                    </div>
                `;
                return;
            }
            
            savedGames.forEach(game => {
                const lionCount = game.animalCounts.lion || 0;
                const antelopeCount = game.animalCounts.antelope || 0;
                
                const gameItem = document.createElement('div');
                gameItem.className = 'list-group-item';
                gameItem.innerHTML = `
                    <div class="d-flex w-100 justify-content-between align-items-center">
                        <div>
                            <h6 class="mb-1">${game.name}</h6>
                            <small class="text-muted">Saved: ${new Date(game.saveDate).toLocaleString()}</small>
                        </div>
                        <div class="btn-group btn-group-sm">
                            <button class="btn btn-outline-primary" onclick="saveManager.loadGame(${game.id}, ${game.iteration})">
                                <i class="bi bi-play-fill"></i> Load
                            </button>
                            <button class="btn btn-outline-danger" onclick="saveManager.deleteSave(${game.id})">
                                <i class="bi bi-trash"></i>
                            </button>
                        </div>
                    </div>
                    <div class="mt-2">
                        <small class="me-2">Iteration: ${game.iteration}</small>
                        <small class="me-2">Lions: ${lionCount}</small>
                        <small>Antelopes: ${antelopeCount}</small>
                    </div>
                `;
                container.appendChild(gameItem);
            });
        } catch (error) {
            uiManager.showErrorMessage(error.message);
        }
    }

    async loadGame(saveId, savedIteration) {
        try {
            // First, quit any existing game
            if (gameState.gameActive) {
                await gameControls.quitGame();
            }
            
            const response = await fetch(`/api/games/load/${saveId}`, {
                method: 'POST'
            });
            
            if (!response.ok) {
                const data = await response.json();
                throw new Error(data.message || GameConstants.Messages.FailedToLoadGame);
            }
            
            const stateResponse = await fetch('/api/games/state');
            if (!stateResponse.ok) {
                throw new Error(GameConstants.Messages.FailedToLoadGame);
            }

            const stateData = await stateResponse.json();
            if (!stateData) {
                throw new Error(GameConstants.Messages.FailedToLoadGame);
            }

            gameState.gameActive = true;
            uiManager.enableGameControls();
            gameState.startGameStatePolling();
            uiManager.updateUI(stateData);
            uiManager.showSuccessMessage(GameConstants.Messages.GameLoaded);
        } catch (error) {
            uiManager.showErrorMessage(error.message);
            gameState.resetGameState();
        }
    }

    async saveGame() {
        try {
            const response = await fetch('/api/games/save', { method: 'POST' });
            if (!response.ok) {
                const data = await response.json();
                throw new Error(data.message || GameConstants.Messages.FailedToSaveGame);
            }
            
            gameState.hasUnsavedChanges = false;
            await this.loadSavedGames(); // Refresh the saves list
            uiManager.showSuccessMessage(GameConstants.Messages.GameSaved);
        } catch (error) {
            uiManager.showErrorMessage(error.message);
        }
    }

    async deleteSave(saveId) {
        if (!confirm(GameConstants.Confirmations.DeleteSave)) {
            return;
        }
        
        try {
            const response = await fetch(`/api/games/saved/${saveId}`, { 
                method: 'DELETE'
            });
            
            if (!response.ok) {
                const data = await response.json();
                throw new Error(data.message || GameConstants.Messages.FailedToDeleteSave);
            }
            
            await this.loadSavedGames(); // Refresh the list
            uiManager.showSuccessMessage(GameConstants.Messages.SaveDeleted);
        } catch (error) {
            uiManager.showErrorMessage(error.message);
        }
    }
}

// Export as global instance
window.saveManager = new SaveManager(); 