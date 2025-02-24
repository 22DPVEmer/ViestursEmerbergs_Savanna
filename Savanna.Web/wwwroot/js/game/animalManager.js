// Animal Management
class AnimalManager {
    async addAnimal(type, isInitialAnimal = false) {
        if (!gameState.gameActive) {
            console.error('Cannot add animal - game is not active');
            uiManager.showErrorMessage('Cannot add animals - game is not active');
            return;
        }

        try {
            console.log(`Adding animal of type: ${type}`);
            const response = await fetch('/api/games/add-animal', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(type)
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || `Failed to add ${type}`);
            }
            
            const data = await response.json();
            console.log('Animal added successfully:', data.message);
            gameState.hasUnsavedChanges = true;

            // Only update game state if not adding initial animals
            if (!isInitialAnimal) {
                console.log('Updating game state...');
                //await updateGameState();
            } else {
                // For initial animals, just get the current state without updating
                const stateResponse = await fetch('/api/games/state');
                if (stateResponse.ok) {
                    const stateData = await stateResponse.json();
                    uiManager.updateUI(stateData);
                }
            }
        } catch (error) {
            console.error('Error adding animal:', error);
            uiManager.showErrorMessage(`Failed to add ${type}: ${error.message}`);
            
            // If we get a "No active game" error, reset the game state
            if (error.message.includes('No active game')) {
                gameState.resetGameState();
            }
        }
    }

    async initializeAnimalButtons() {
        try {
            const response = await fetch('/api/games/animal-types');
            if (!response.ok) throw new Error('Failed to get animal types');
            
            const animalTypes = await response.json();
            const buttonContainer = document.getElementById('animalControls');
            
            if (buttonContainer) {
                buttonContainer.innerHTML = ''; // Clear existing buttons
                
                animalTypes.forEach(type => {
                    const button = document.createElement('button');
                    button.id = `add${type}`;
                    button.className = `btn btn-outline-${uiManager.animalColors[type]}`;
                    button.disabled = true;
                    button.innerHTML = `<i class="bi bi-plus-circle"></i> Add ${type}`;
                    button.addEventListener('click', () => {
                        if (gameState.gameActive) {
                            this.addAnimal(type).catch(error => {
                                console.error(`Failed to add ${type}:`, error);
                            });
                        }
                    });
                    buttonContainer.appendChild(button);
                });
            }
        } catch (error) {
            console.error('Error initializing animal buttons:', error);
            uiManager.showErrorMessage('Failed to initialize animal controls. Please refresh the page.');
        }
    }

    async initializeAnimalStats() {
        try {
            const response = await fetch('/api/games/animal-types');
            if (!response.ok) throw new Error('Failed to get animal types');
            
            const animalTypes = await response.json();
            const statsContainer = document.getElementById('animalStats');
            
            if (statsContainer) {
                statsContainer.innerHTML = ''; // Clear existing stats
                
                animalTypes.forEach(type => {
                    const statItem = document.createElement('div');
                    statItem.className = 'list-group-item d-flex justify-content-between align-items-center';
                    statItem.innerHTML = `
                        <span>${type}s</span>
                        <span id="${type.toLowerCase()}Count" class="badge bg-${uiManager.animalColors[type]} rounded-pill">0</span>
                    `;
                    statsContainer.appendChild(statItem);
                });
            }
        } catch (error) {
            console.error('Error initializing animal statistics:', error);
        }
    }
}

// Export as global instance
window.animalManager = new AnimalManager(); 