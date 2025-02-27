// UI Management
class UIManager {
    constructor() {
        this.animalIcons = GameConstants.Icons;
        this.animalText = GameConstants.TextSymbols;
        this.animalColors = GameConstants.Colors;
        this.selectedAnimal = null;
        this.currentAnimals = [];
    }

    showErrorMessage(message) {
        const errorDiv = document.createElement('div');
        errorDiv.className = GameConstants.CSS.ALERT_ERROR;
        errorDiv.innerHTML = `
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        `;
        
        const container = document.querySelector('.card-body') || document.body;
        container.insertAdjacentElement('afterbegin', errorDiv);

        setTimeout(() => {
            errorDiv.remove();
        }, GameConstants.GameDefaults.ALERT_TIMEOUT);
    }

    showSuccessMessage(message) {
        const successDiv = document.createElement('div');
        successDiv.className = GameConstants.CSS.ALERT_SUCCESS;
        successDiv.innerHTML = `
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        `;
        
        const container = document.querySelector('.card-body') || document.body;
        container.insertAdjacentElement('afterbegin', successDiv);

        setTimeout(() => {
            successDiv.remove();
        }, GameConstants.GameDefaults.ALERT_TIMEOUT);
    }

    updateUI(gameState) {
        // Update iteration counter
        document.getElementById(GameConstants.ElementIds.ITERATION_COUNTER).textContent = gameState.iteration;

        // Update all animal counts
        Object.keys(this.animalIcons).forEach(type => {
            const countElement = document.getElementById(`${type.toLowerCase()}Count`);
            if (countElement) {
                countElement.textContent = gameState.animalCounts[type] || 0;
            }
        });

        // Update game grid
        this.updateGameGrid(gameState.animals);
    }

    updateGameGrid(animals) {
        // Clear previous selection highlight if animal moved
        if (this.selectedAnimal) {
            const selectedAnimal = animals.find(a => a.id === this.selectedAnimal.id);
            if (selectedAnimal && 
                (selectedAnimal.x !== this.selectedAnimal.x || 
                 selectedAnimal.y !== this.selectedAnimal.y)) {
                this.selectedAnimal = selectedAnimal; // Update position
            }
        }

        this.currentAnimals = animals; // Store current animals for selection
        const cells = document.querySelectorAll(`.${GameConstants.CSS.GAME_CELL}`);
        if (!cells.length) return;

        // Clear all cells first
        cells.forEach(cell => {
            cell.innerHTML = '';
            cell.className = GameConstants.CSS.GAME_CELL;
            cell.dataset.animalType = '';
            cell.dataset.animalId = '';
        });

        // Update cells with animals
        animals.forEach(animal => {
            if (!animal.isAlive) return;
            
            const index = animal.y * GameConstants.GameDefaults.GRID_SIZE + animal.x;
            const cell = cells[index];
            if (cell) {
                cell.dataset.animalType = animal.type;
                cell.dataset.animalId = animal.id;
                
                // Set class and check if this is the selected animal
                const isSelected = this.selectedAnimal?.id === animal.id;
                cell.className = `${GameConstants.CSS.GAME_CELL} ${animal.type.toLowerCase()}${isSelected ? ' selected' : ''}`;
                
                // Create animal icon/text element
                const animalSymbol = document.createElement('div');
                animalSymbol.textContent = gameState.displayMode === GameConstants.GameDefaults.DEFAULT_DISPLAY_MODE 
                    ? this.animalIcons[animal.type] 
                    : this.animalText[animal.type];
                cell.appendChild(animalSymbol);

                // Create health bar with proper health calculation
                const healthBar = document.createElement('div');
                healthBar.className = 'health-bar';
                const healthBarFill = document.createElement('div');
                healthBarFill.className = 'health-bar-fill';
                
                // Get max health based on animal type
                const maxHealth = GameConstants.Health.MaxHealth[animal.type.toUpperCase()];
                const healthPercentage = (animal.health / maxHealth) * 100;
                healthBarFill.style.width = `${Math.max(0, Math.min(100, healthPercentage))}%`;
                
                healthBar.appendChild(healthBarFill);
                cell.appendChild(healthBar);
                
                // Add click handler
                cell.addEventListener('click', () => this.handleAnimalClick(animal));
            }
        });

        // Update details panel if selected animal still exists
        if (this.selectedAnimal) {
            const animal = animals.find(a => a.id === this.selectedAnimal.id);
            if (animal && animal.isAlive) {
                this.updateAnimalDetails(animal);
            } else {
                this.clearSelection();
            }
        }
    }

    handleAnimalClick(animal) {
        // Clear previous selection
        const prevSelectedCell = document.querySelector('.game-cell.selected');
        if (prevSelectedCell) {
            prevSelectedCell.classList.remove('selected');
        }

        // Update selection
        if (this.selectedAnimal?.id === animal.id) {
            this.clearSelection();
        } else {
            this.selectedAnimal = { ...animal }; // Store a copy of the animal data
            const cell = document.querySelector(`[data-animal-id="${animal.id}"]`);
            if (cell) {
                cell.classList.add('selected');
            }
            this.updateAnimalDetails(animal);
        }
    }

    updateAnimalDetails(animal) {
        const detailsCard = document.getElementById('animalDetails');
        const typeElement = document.getElementById('animalType');
        const ageElement = document.getElementById('animalAge');
        const healthElement = document.getElementById('animalHealth');
        const offspringElement = document.getElementById('animalOffspring');

        if (!animal) {
            detailsCard.style.display = 'none';
            return;
        }

        detailsCard.style.display = 'block';
        
        // Update species with proper styling
        typeElement.textContent = animal.type;
        typeElement.className = `stat-value species ${animal.type.toLowerCase()}`;
        
        // Update age
        ageElement.textContent = animal.age;
        
        // Update health with percentage-based background
        const maxHealth = GameConstants.Health.MaxHealth[animal.type.toUpperCase()];
        const healthPercentage = (animal.health / maxHealth) * 100;
        healthElement.textContent = `${animal.health}/${maxHealth}`;
        healthElement.style.setProperty('--health-percentage', `${healthPercentage}%`);
        
        // Update offspring count
        offspringElement.textContent = animal.offspringCount;
    }

    clearSelection() {
        this.selectedAnimal = null;
        const detailsPanel = document.getElementById('animalDetails');
        if (detailsPanel) {
            detailsPanel.classList.remove('visible');
        }
    }

    resetUI() {
        const startButton = document.getElementById(GameConstants.ElementIds.START_GAME);
        const quitButton = document.getElementById(GameConstants.ElementIds.QUIT_GAME);
        const saveButton = document.getElementById(GameConstants.ElementIds.SAVE_GAME);
        const pauseButton = document.getElementById(GameConstants.ElementIds.PAUSE_GAME);

        if (startButton) startButton.disabled = false;
        if (quitButton) quitButton.disabled = true;
        if (saveButton) saveButton.disabled = true;
        if (pauseButton) pauseButton.disabled = true;

        // Disable all animal buttons
        document.querySelectorAll(GameConstants.CSS.BUTTON_ADD_PREFIX).forEach(button => {
            button.disabled = true;
        });

        this.updatePauseButtonText();

        // Clear the game grid
        const cells = document.querySelectorAll(`.${GameConstants.CSS.GAME_CELL}`);
        cells.forEach(cell => {
            cell.textContent = '';
            cell.className = GameConstants.CSS.GAME_CELL;
            cell.dataset.animalType = '';
        });

        // Reset all animal counters
        Object.keys(this.animalIcons).forEach(type => {
            const countElement = document.getElementById(`${type.toLowerCase()}Count`);
            if (countElement) countElement.textContent = '0';
        });

        document.getElementById(GameConstants.ElementIds.ITERATION_COUNTER).textContent = '0';
        this.clearSelection();
    }

    updatePauseButtonText() {
        const pauseButton = document.getElementById(GameConstants.ElementIds.PAUSE_GAME);
        if (pauseButton) {
            pauseButton.textContent = gameState.isPaused 
                ? GameConstants.Messages.Buttons.RESUME_GAME 
                : GameConstants.Messages.Buttons.PAUSE_GAME;
        }
    }

    enableGameControls() {
        const startButton = document.getElementById(GameConstants.ElementIds.START_GAME);
        const quitButton = document.getElementById(GameConstants.ElementIds.QUIT_GAME);
        const saveButton = document.getElementById(GameConstants.ElementIds.SAVE_GAME);
        const pauseButton = document.getElementById(GameConstants.ElementIds.PAUSE_GAME);

        if (startButton) startButton.disabled = true;
        if (quitButton) quitButton.disabled = false;
        if (saveButton) saveButton.disabled = false;
        if (pauseButton) pauseButton.disabled = false;

        // Enable all animal buttons
        document.querySelectorAll(GameConstants.CSS.BUTTON_ADD_PREFIX).forEach(button => {
            button.disabled = false;
        });

        this.updatePauseButtonText();
    }
}

// Export as global instance
window.uiManager = new UIManager(); 