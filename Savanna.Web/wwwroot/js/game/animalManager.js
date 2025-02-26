// Animal Management
class AnimalManager {
    async addAnimal(type, isInitialAnimal = false) {
        if (!gameState.gameActive) {
            console.error(GameConstants.UI.Messages.Error.GAME_NOT_ACTIVE);
            uiManager.showErrorMessage(GameConstants.UI.Messages.Error.GAME_NOT_ACTIVE);
            return;
        }

        try {
            console.log(GameConstants.UI.Messages.Console.ADDING_ANIMAL(type));
            const response = await fetch(GameConstants.Api.Endpoints.ADD_ANIMAL, {
                method: GameConstants.Api.Methods.POST,
                headers: { [GameConstants.Api.Headers.CONTENT_TYPE]: GameConstants.Api.Headers.JSON },
                body: JSON.stringify(type)
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || GameConstants.UI.Messages.Error.FAILED_TO_ADD_ANIMAL);
            }
            
            const data = await response.json();
            console.log(GameConstants.UI.Messages.Console.ANIMAL_ADDED(data.message));
            gameState.hasUnsavedChanges = true;

            // Only update game state if not adding initial animals
            if (!isInitialAnimal) {
                console.log(GameConstants.UI.Messages.Console.UPDATING_STATE);
                //await updateGameState();
            } else {
                // For initial animals, just get the current state without updating
                const stateResponse = await fetch(GameConstants.Api.Endpoints.STATE);
                if (stateResponse.ok) {
                    const stateData = await stateResponse.json();
                    uiManager.updateUI(stateData);
                }
            }
        } catch (error) {
            console.error(GameConstants.UI.Messages.Console.ERROR_ADDING(type, error));
            uiManager.showErrorMessage(GameConstants.UI.Messages.Error.FAILED_TO_ADD_ANIMAL);
            
            // If we get a "No active game" error, reset the game state
            if (error.message.includes(GameConstants.UI.Messages.Error.NO_ACTIVE_GAME)) {
                gameState.resetGameState();
            }
        }
    }

    async initializeAnimalButtons() {
        try {
            const response = await fetch(GameConstants.Api.Endpoints.ANIMAL_TYPES);
            if (!response.ok) throw new Error(GameConstants.UI.Messages.Error.FAILED_TO_INIT_CONTROLS);
            
            const animalTypes = await response.json();
            const buttonContainer = document.getElementById(GameConstants.UI.Elements.IDs.ANIMAL_CONTROLS);
            
            if (buttonContainer) {
                buttonContainer.innerHTML = ''; // Clear existing buttons
                
                animalTypes.forEach(type => {
                    const button = document.createElement('button');
                    button.id = `add${type}`;
                    button.className = GameConstants.UI.Elements.Classes.BUTTON.replace('{0}', uiManager.animalColors[type]);
                    button.disabled = true;
                    button.innerHTML = `${GameConstants.UI.Elements.Icons.ADD} ${GameConstants.UI.Messages.Buttons.ADD_ANIMAL(type)}`;
                    button.addEventListener('click', () => {
                        if (gameState.gameActive) {
                            this.addAnimal(type).catch(error => {
                                console.error(GameConstants.UI.Messages.Console.ERROR_ADDING(type, error));
                            });
                        }
                    });
                    buttonContainer.appendChild(button);
                });
            }
        } catch (error) {
            console.error(GameConstants.UI.Messages.Error.GENERIC_ERROR('initializing animal buttons', error));
            uiManager.showErrorMessage(GameConstants.UI.Messages.Error.FAILED_TO_INIT_CONTROLS);
        }
    }

    async initializeAnimalStats() {
        try {
            const response = await fetch(GameConstants.Api.Endpoints.ANIMAL_TYPES);
            if (!response.ok) throw new Error(GameConstants.UI.Messages.Error.FAILED_TO_INIT_STATS);
            
            const animalTypes = await response.json();
            const statsContainer = document.getElementById(GameConstants.UI.Elements.IDs.ANIMAL_STATS);
            
            if (statsContainer) {
                statsContainer.innerHTML = ''; // Clear existing stats
                
                animalTypes.forEach(type => {
                    const statItem = document.createElement('div');
                    statItem.className = GameConstants.UI.Elements.Classes.LIST_GROUP_ITEM;
                    statItem.innerHTML = `
                        <span>${type}s</span>
                        <span id="${type.toLowerCase()}Count" class="${GameConstants.UI.Elements.Classes.BADGE.replace('{0}', uiManager.animalColors[type])}">0</span>
                    `;
                    statsContainer.appendChild(statItem);
                });
            }
        } catch (error) {
            console.error(GameConstants.UI.Messages.Error.GENERIC_ERROR('initializing animal statistics', error));
            uiManager.showErrorMessage(GameConstants.UI.Messages.Error.FAILED_TO_INIT_STATS);
        }
    }
}

// Export as global instance
window.animalManager = new AnimalManager(); 