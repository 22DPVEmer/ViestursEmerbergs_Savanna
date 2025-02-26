// UI Management
class UIManager {
    constructor() {
        this.animalIcons = GameConstants.Icons;
        this.animalText = GameConstants.TextSymbols;
        this.animalColors = GameConstants.Colors;
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
        Object.keys(gameState.animalCounts).forEach(type => {
            const countElement = document.getElementById(`${type.toLowerCase()}Count`);
            if (countElement) {
                countElement.textContent = gameState.animalCounts[type] || 0;
            }
        });

        // Update game grid
        this.updateGameGrid(gameState.animals);
    }

    updateGameGrid(animals) {
        const cells = document.querySelectorAll(`.${GameConstants.CSS.GAME_CELL}`);
        if (!cells.length) return;

        cells.forEach(cell => {
            cell.textContent = '';
            cell.className = GameConstants.CSS.GAME_CELL;
            cell.dataset.animalType = '';
        });

        animals.forEach(animal => {
            const index = animal.y * GameConstants.GameDefaults.GRID_SIZE + animal.x;
            const cell = cells[index];
            if (cell) {
                cell.dataset.animalType = animal.type;
                cell.className = `${GameConstants.CSS.GAME_CELL} ${animal.type.toLowerCase()}`;
                cell.textContent = gameState.displayMode === GameConstants.GameDefaults.DEFAULT_DISPLAY_MODE 
                    ? this.animalIcons[animal.type] 
                    : this.animalText[animal.type];
            }
        });
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