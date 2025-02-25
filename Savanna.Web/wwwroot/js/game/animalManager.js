// Animal Management
class AnimalManager {
    constructor() {
        this.animalTypes = [];
        this.initializeAnimalTypes();
    }

    async initializeAnimalTypes() {
        try {
            const response = await fetch(GameConstants.Api.Endpoints.ANIMAL_TYPES, {
                method: GameConstants.Api.Methods.GET
            });

            if (!response.ok) {
                throw new Error(GameConstants.Messages.Error.FAILED_TO_INITIALIZE);
            }

            this.animalTypes = await response.json();
            this.setupAnimalControls();
        } catch (error) {
            uiManager.showErrorMessage(GameConstants.Messages.Error.FAILED_TO_INITIALIZE);
        }
    }

    setupAnimalControls() {
        const container = document.getElementById('animalControls');
        if (!container) return;

        container.innerHTML = '';
        
        this.animalTypes.forEach(type => {
            const button = document.createElement('button');
            button.className = `btn btn-${GameConstants.Animals.Colors[type.toUpperCase()]} me-2`;
            button.innerHTML = `${this.getAnimalIcon(type)} Add ${type}`;
            button.onclick = () => this.addAnimal(type);
            container.appendChild(button);
        });
    }

    getAnimalIcon(type) {
        const upperType = type.toUpperCase();
        return gameState.displayMode === GameConstants.Game.Display.ICONS
            ? GameConstants.Animals.Icons[upperType]
            : GameConstants.Animals.Text[upperType];
    }

    async addAnimal(type, isInitial = false) {
        if (!gameState.gameActive && !isInitial) {
            uiManager.showErrorMessage(GameConstants.Messages.Error.NO_ACTIVE_GAME);
            return;
        }

        try {
            const response = await fetch(GameConstants.Api.Endpoints.ADD_ANIMAL, {
                method: GameConstants.Api.Methods.POST,
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ animalType: type })
            });

            if (!response.ok) {
                throw new Error(GameConstants.Messages.Error.FAILED_TO_ADD_ANIMAL);
            }

            if (!isInitial) {
                gameState.hasUnsavedChanges = true;
                uiManager.showSuccessMessage(GameConstants.Messages.Success.ANIMAL_ADDED);
            }
        } catch (error) {
            uiManager.showErrorMessage(GameConstants.Messages.Error.FAILED_TO_ADD_ANIMAL);
        }
    }

    async initializeAnimalStats() {
        try {
            const response = await fetch('/api/games/animal-types');
            if (!response.ok) throw new Error(GameConstants.Messages.FailedToInitControls);
            
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
            uiManager.showErrorMessage(GameConstants.Messages.FailedToInitControls);
        }
    }
}

// Export as global instance
window.animalManager = new AnimalManager(); 