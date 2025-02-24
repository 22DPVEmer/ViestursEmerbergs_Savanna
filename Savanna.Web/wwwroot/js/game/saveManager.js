// Save Management
class SaveManager {
    async loadSavedGames() {
        try {
            console.log('Loading saved games...');
            const response = await fetch('/api/games/saved');
            console.log('Response status:', response.status);
            
            if (!response.ok) {
                if (response.status === 401) {
                    console.log('Not authenticated, skipping saves display');
                    return; // Not logged in, just don't show saves
                }
                throw new Error('Failed to load saved games');
            }
            
            const savedGames = await response.json();
            console.log('Loaded saved games:', savedGames);
            
            const container = document.getElementById('savedGames');
            if (!container) {
                console.error('Saved games container not found');
                return;
            }
            
            container.innerHTML = ''; // Clear existing games
            
            if (savedGames.length === 0) {
                container.innerHTML = `
                    <div class="list-group-item text-center text-muted">
                        No saved games found
                    </div>
                `;
                return;
            }
            
            savedGames.forEach(game => {
                console.log('Processing game:', game);
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
            console.error('Error loading saved games:', error);
            uiManager.showErrorMessage('Failed to load saved games: ' + error.message);
        }
    }

    async loadGame(saveId, savedIteration) {
        try {
            console.log('Loading game:', saveId, 'with iteration:', savedIteration);
            
            // First, quit any existing game
            if (gameState.gameActive) {
                await gameControls.quitGame();
            }
            
            const response = await fetch(`/api/games/load/${saveId}`, {
                method: 'POST'
            });
            
            if (!response.ok) {
                const data = await response.json();
                throw new Error(data.message || 'Failed to load game');
            }
            
            // Wait for game to initialize with retries
            let retries = 5;
            let stateData = null;
            let lastError = null;
            
            while (retries > 0 && !stateData) {
                try {
                    await new Promise(resolve => setTimeout(resolve, 1000));
                    console.log(`Verifying loaded game state (${retries} attempts left)...`);
                    
                    const stateResponse = await fetch('/api/games/state');
                    console.log('State response:', stateResponse.status);
                    
                    if (stateResponse.ok) {
                        stateData = await stateResponse.json();
                        if (stateData) {
                            console.log('Loaded game state verified:', stateData);
                            break;
                        }
                    } else {
                        lastError = `State verification failed with status ${stateResponse.status}`;
                    }
                    
                    retries--;
                } catch (error) {
                    console.warn('State verification attempt failed:', error);
                    lastError = error.message;
                    retries--;
                }
            }
            
            if (!stateData) {
                throw new Error(lastError || 'Could not verify loaded game state');
            }

            gameState.gameActive = true;
            uiManager.enableGameControls();
            gameState.startGameStatePolling();
            uiManager.updateUI(stateData);
            uiManager.showSuccessMessage('Game loaded successfully');
        } catch (error) {
            console.error('Error loading game:', error);
            uiManager.showErrorMessage('Failed to load game: ' + error.message);
            gameState.resetGameState();
        }
    }

    async saveGame() {
        try {
            const response = await fetch('/api/games/save', { method: 'POST' });
            if (!response.ok) {
                const data = await response.json();
                throw new Error(data.message || 'Failed to save game');
            }
            
            const data = await response.json();
            console.log('Save response:', data);
            
            gameState.hasUnsavedChanges = false;
            await this.loadSavedGames(); // Refresh the saves list
            uiManager.showSuccessMessage('Game saved successfully');
        } catch (error) {
            console.error('Error saving game:', error);
            uiManager.showErrorMessage('Failed to save game: ' + error.message);
        }
    }

    async deleteSave(saveId) {
        if (!confirm('Are you sure you want to delete this saved game?')) {
            return;
        }
        
        try {
            const response = await fetch(`/api/games/saved/${saveId}`, { 
                method: 'DELETE'
            });
            
            if (!response.ok) {
                const data = await response.json();
                throw new Error(data.message || 'Failed to delete save');
            }
            
            await this.loadSavedGames(); // Refresh the list
            uiManager.showSuccessMessage('Game save deleted successfully');
        } catch (error) {
            console.error('Error deleting save:', error);
            uiManager.showErrorMessage('Failed to delete save: ' + error.message);
        }
    }
}

// Export as global instance
window.saveManager = new SaveManager(); 