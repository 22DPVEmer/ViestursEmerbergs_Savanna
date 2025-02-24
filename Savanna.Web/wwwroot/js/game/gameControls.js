// Game Controls
class GameControls {
    async startGame() {
        try {
            console.log('Starting game...');
            const response = await fetch('/api/games/start', { method: 'POST' });
            if (!response.ok) throw new Error('Failed to start game');
            
            const data = await response.json();
            console.log('Start game response:', data.message);
            
            // Wait for game to initialize with retries
            let retries = 5;
            let stateData = null;
            let lastError = null;
            
            while (retries > 0 && !stateData) {
                try {
                    await new Promise(resolve => setTimeout(resolve, 1000));
                    console.log(`Attempting to verify game state (${retries} attempts left)...`);
                    
                    const stateResponse = await fetch('/api/games/state');
                    console.log('State response status:', stateResponse.status);
                    
                    if (stateResponse.ok) {
                        stateData = await stateResponse.json();
                        if (stateData) {
                            console.log('Game state verified:', stateData);
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
                throw new Error(lastError || 'Could not verify game state after multiple attempts');
            }
            
            gameState.gameActive = true;
            uiManager.enableGameControls();

            // Add initial animals without starting polling
            console.log('Adding initial animals...');
            await animalManager.addAnimal('Lion', true);
            await animalManager.addAnimal('Antelope', true);
            console.log('Initial animals added successfully');

            // Start polling after initial animals are added
            gameState.startGameStatePolling();
        } catch (error) {
            console.error('Error starting game:', error);
            uiManager.showErrorMessage('Failed to start game: ' + error.message);
            gameState.resetGameState();
        }
    }

    async quitGame() {
        try {
            const response = await fetch('/api/games/quit', { method: 'POST' });
            if (!response.ok) throw new Error('Failed to quit game');
            
            const data = await response.json();
            console.log(data.message);
            
            gameState.gameActive = false;
            gameState.stopGameStatePolling();
            gameState.resetGameState();
        } catch (error) {
            console.error('Error quitting game:', error);
            uiManager.showErrorMessage('Failed to quit game. Please try again.');
        }
    }

    async togglePause() {
        try {
            const response = await fetch('/api/games/toggle-pause', { method: 'POST' });
            if (!response.ok) throw new Error('Failed to toggle pause state');
            
            const data = await response.json();
            gameState.isPaused = data.isPaused;
            uiManager.updatePauseButtonText();
        } catch (error) {
            console.error('Error toggling pause state:', error);
            uiManager.showErrorMessage('Failed to toggle pause state. Please try again.');
        }
    }
}

// Export as global instance
window.gameControls = new GameControls(); 