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

        this.messageTimeout = null;
        this.setupEventListeners();
    }

    setupEventListeners() {
        const displayModeToggle = document.getElementById('displayModeToggle');
        if (displayModeToggle) {
            displayModeToggle.addEventListener('change', () => {
                gameState.displayMode = displayModeToggle.checked 
                    ? GameConstants.Game.Display.ICONS 
                    : GameConstants.Game.Display.TEXT;
                localStorage.setItem(
                    GameConstants.LocalStorage.Keys.DISPLAY_MODE, 
                    gameState.displayMode
                );
                this.updateGameGrid();
            });
        }
    }

    showMessage(message, isError = false) {
        const messageElement = document.getElementById('gameMessage');
        if (!messageElement) return;

        messageElement.textContent = message;
        messageElement.className = `alert ${isError ? 'alert-danger' : 'alert-info'} mt-3`;
        messageElement.style.display = 'block';

        if (this.messageTimeout) {
            clearTimeout(this.messageTimeout);
        }

        this.messageTimeout = setTimeout(() => {
            messageElement.style.display = 'none';
        }, GameConstants.Game.Intervals.MESSAGE_DISPLAY);
    }

    showErrorMessage(message) {
        this.showMessage(message, true);
    }

    showSuccessMessage(message) {
        this.showMessage(message, false);
    }

    updateUI(gameState) {
        this.updateGameGrid(gameState);
        this.updateStatistics(gameState);
        this.updateControls(gameState);
    }

    updateGameGrid(gameState) {
        const grid = document.getElementById('gameGrid');
        if (!grid) return;

        grid.innerHTML = '';
        grid.style.gridTemplateColumns = `repeat(${GameConstants.Game.Grid.WIDTH}, 1fr)`;

        for (let i = 0; i < GameConstants.Game.Grid.TOTAL_CELLS; i++) {
            const cell = document.createElement('div');
            cell.className = GameConstants.CSS.Classes.GAME_CELL;
            grid.appendChild(cell);
        }

        if (!gameState?.animals) return;

        gameState.animals.forEach(animal => {
            if (!animal.isAlive) return;

            const cell = grid.children[animal.position];
            if (cell) {
                const type = animal.animalType.toUpperCase();
                cell.className = `${GameConstants.CSS.Classes.GAME_CELL} ${GameConstants.CSS.Classes[type]}`;
                cell.textContent = this.getAnimalDisplay(animal.animalType);
            }
        });
    }

    getAnimalDisplay(type) {
        const upperType = type.toUpperCase();
        return gameState.displayMode === GameConstants.Game.Display.ICONS
            ? GameConstants.Animals.Icons[upperType]
            : GameConstants.Animals.Text[upperType];
    }

    updateStatistics(gameState) {
        if (!gameState) return;

        const stats = {
            [GameConstants.Animals.Types.LION]: 0,
            [GameConstants.Animals.Types.ANTELOPE]: 0,
            [GameConstants.Animals.Types.TIGER]: 0,
            [GameConstants.Animals.Types.ZEBRA]: 0
        };

        gameState.animals.forEach(animal => {
            if (animal.isAlive) {
                stats[animal.animalType]++;
            }
        });

        Object.entries(stats).forEach(([type, count]) => {
            const element = document.getElementById(`${type.toLowerCase()}Count`);
            if (element) {
                element.textContent = count;
            }
        });

        const iterationElement = document.getElementById('iteration');
        if (iterationElement) {
            iterationElement.textContent = gameState.currentIteration;
        }
    }

    updateControls(gameState) {
        const startButton = document.getElementById('startGame');
        const quitButton = document.getElementById('quitGame');
        const pauseButton = document.getElementById('togglePause');
        const saveButton = document.getElementById('saveGame');
        const animalControls = document.getElementById('animalControls');

        if (startButton) startButton.style.display = gameState.gameActive ? 'none' : 'block';
        if (quitButton) quitButton.style.display = gameState.gameActive ? 'block' : 'none';
        if (pauseButton) {
            pauseButton.style.display = gameState.gameActive ? 'block' : 'none';
            this.updatePauseButtonText();
        }
        if (saveButton) saveButton.style.display = gameState.gameActive ? 'block' : 'none';
        if (animalControls) animalControls.style.display = gameState.gameActive ? 'block' : 'none';
    }

    updatePauseButtonText() {
        const pauseButton = document.getElementById('togglePause');
        if (pauseButton) {
            pauseButton.textContent = gameState.isPaused ? '▶ Resume' : '⏸ Pause';
        }
    }

    enableGameControls() {
        const buttons = document.querySelectorAll('#animalControls button');
        buttons.forEach(button => button.disabled = false);
    }

    resetUI() {
        this.updateGameGrid({ animals: [] });
        this.updateStatistics({ animals: [] });
        this.updateControls({ gameActive: false });
    }
}

// Export as global instance
window.uiManager = new UIManager(); 