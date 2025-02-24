// UI Management
class UIManager {
    constructor() {
        this.animalIcons = {
            'Lion': '🦁',
            'Antelope': '🦌',
            'Tiger': '🐯',
            'Zebra': '🦓'
        };

        this.animalText = {
            'Lion': 'L',
            'Antelope': 'A',
            'Tiger': 'T',
            'Zebra': 'Z'
        };

        this.animalColors = {
            'Lion': 'danger',
            'Antelope': 'success',
            'Tiger': 'warning',
            'Zebra': 'secondary'
        };
    }

    showErrorMessage(message) {
        const errorDiv = document.createElement('div');
        errorDiv.className = 'alert alert-danger alert-dismissible fade show';
        errorDiv.innerHTML = `
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        `;
        
        const container = document.querySelector('.card-body') || document.body;
        container.insertAdjacentElement('afterbegin', errorDiv);

        setTimeout(() => {
            errorDiv.remove();
        }, 5000);
    }

    showSuccessMessage(message) {
        const successDiv = document.createElement('div');
        successDiv.className = 'alert alert-success alert-dismissible fade show';
        successDiv.innerHTML = `
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        `;
        
        const container = document.querySelector('.card-body') || document.body;
        container.insertAdjacentElement('afterbegin', successDiv);

        setTimeout(() => {
            successDiv.remove();
        }, 5000);
    }

    updateUI(gameState) {
        // Update iteration counter
        document.getElementById('iterationCounter').textContent = gameState.iteration;

        // First reset all counts to 0
        const animalTypes = Object.keys(this.animalIcons);
        animalTypes.forEach(type => {
            const countElement = document.getElementById(`${type.toLowerCase()}Count`);
            if (countElement) {
                countElement.textContent = '0';
            }
        });

        // Count only living animals
        const livingAnimals = gameState.animals.filter(animal => animal.isAlive);
        const animalCounts = {};
        livingAnimals.forEach(animal => {
            animalCounts[animal.type] = (animalCounts[animal.type] || 0) + 1;
        });

        // Update the counts based on living animals
        Object.entries(animalCounts).forEach(([type, count]) => {
            const countElement = document.getElementById(`${type.toLowerCase()}Count`);
            if (countElement) {
                countElement.textContent = count;
            }
        });

        // Update game grid
        this.updateGameGrid(gameState.animals);
    }

    updateGameGrid(animals) {
        // First clear all cells
        const cells = document.querySelectorAll('.game-cell');
        cells.forEach(cell => {
            cell.textContent = '';
            cell.className = 'game-cell';
            cell.dataset.animalType = '';
        });

        // Then only update cells with living animals
        animals.filter(animal => animal.isAlive).forEach(animal => {
            const index = animal.y * 20 + animal.x;
            const cell = cells[index];
            if (cell) {
                cell.dataset.animalType = animal.type;
                cell.className = `game-cell ${animal.type.toLowerCase()}`;
                cell.textContent = gameState.displayMode === 'icons' 
                    ? this.animalIcons[animal.type] 
                    : this.animalText[animal.type];
            }
        });
    }

    resetUI() {
        const startButton = document.getElementById('startGame');
        const quitButton = document.getElementById('quitGame');
        const saveButton = document.getElementById('saveGame');
        const pauseButton = document.getElementById('pauseGame');

        if (startButton) startButton.disabled = false;
        if (quitButton) quitButton.disabled = true;
        if (saveButton) saveButton.disabled = true;
        if (pauseButton) pauseButton.disabled = true;

        // Disable all animal buttons
        document.querySelectorAll('[id^="add"]').forEach(button => {
            button.disabled = true;
        });

        this.updatePauseButtonText();

        // Clear the game grid
        const cells = document.querySelectorAll('.game-cell');
        cells.forEach(cell => {
            cell.textContent = '';
            cell.className = 'game-cell';
            cell.dataset.animalType = '';
        });

        // Reset all animal counters
        Object.keys(this.animalIcons).forEach(type => {
            const countElement = document.getElementById(`${type.toLowerCase()}Count`);
            if (countElement) countElement.textContent = '0';
        });

        document.getElementById('iterationCounter').textContent = '0';
    }

    updatePauseButtonText() {
        const pauseButton = document.getElementById('pauseGame');
        if (pauseButton) {
            pauseButton.textContent = gameState.isPaused ? 'Resume Game' : 'Pause Game';
        }
    }

    enableGameControls() {
        const startButton = document.getElementById('startGame');
        const quitButton = document.getElementById('quitGame');
        const saveButton = document.getElementById('saveGame');
        const pauseButton = document.getElementById('pauseGame');

        if (startButton) startButton.disabled = true;
        if (quitButton) quitButton.disabled = false;
        if (saveButton) saveButton.disabled = false;
        if (pauseButton) pauseButton.disabled = false;

        // Enable all animal buttons
        document.querySelectorAll('[id^="add"]').forEach(button => {
            button.disabled = false;
        });

        this.updatePauseButtonText();
    }
}

// Export as global instance
window.uiManager = new UIManager(); 