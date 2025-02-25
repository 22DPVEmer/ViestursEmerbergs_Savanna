// Save Management
class SaveManager {
    constructor() {
        this.savedGames = [];
    }

    async loadSavedGames() {
        try {
            const response = await fetch(GameConstants.Api.Endpoints.SAVED, {
                method: GameConstants.Api.Methods.GET
            });

            if (!response.ok) {
                throw new Error(GameConstants.Messages.Error.FAILED_TO_LOAD);
            }

            const games = await response.json();
            this.savedGames = games;

            if (games.length === 0) {
                uiManager.showMessage(GameConstants.Messages.Error.NO_SAVED_GAMES);
                return;
            }

            this.displaySavedGames(games);
        } catch (error) {
            uiManager.showErrorMessage(GameConstants.Messages.Error.FAILED_TO_LOAD);
        }
    }

    async saveGame(saveName) {
        try {
            const response = await fetch(GameConstants.Api.Endpoints.SAVE, {
                method: GameConstants.Api.Methods.POST,
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ name: saveName })
            });

            if (!response.ok) {
                throw new Error(GameConstants.Messages.Error.FAILED_TO_SAVE);
            }

            gameState.hasUnsavedChanges = false;
            uiManager.showSuccessMessage(GameConstants.Messages.Success.GAME_SAVED);
            await this.loadSavedGames();
        } catch (error) {
            uiManager.showErrorMessage(GameConstants.Messages.Error.FAILED_TO_SAVE);
        }
    }

    async loadGame(saveId) {
        try {
            if (gameState.hasUnsavedChanges && 
                !confirm(GameConstants.Messages.Confirm.UNSAVED_CHANGES)) {
                return;
            }

            const response = await fetch(`${GameConstants.Api.Endpoints.LOAD}/${saveId}`, {
                method: GameConstants.Api.Methods.POST
            });

            if (!response.ok) {
                throw new Error(GameConstants.Messages.Error.FAILED_TO_LOAD);
            }

            gameState.gameActive = true;
            gameState.hasUnsavedChanges = false;
            gameState.startGameStatePolling();
            uiManager.showSuccessMessage(GameConstants.Messages.Success.GAME_LOADED);
        } catch (error) {
            uiManager.showErrorMessage(GameConstants.Messages.Error.FAILED_TO_LOAD);
        }
    }

    async deleteSave(saveId) {
        try {
            if (!confirm(GameConstants.Messages.Confirm.DELETE_SAVE)) {
                return;
            }

            const response = await fetch(`${GameConstants.Api.Endpoints.LOAD}/${saveId}`, {
                method: GameConstants.Api.Methods.DELETE
            });

            if (!response.ok) {
                throw new Error(GameConstants.Messages.Error.FAILED_TO_DELETE);
            }

            uiManager.showSuccessMessage(GameConstants.Messages.Success.SAVE_DELETED);
            await this.loadSavedGames();
        } catch (error) {
            uiManager.showErrorMessage(GameConstants.Messages.Error.FAILED_TO_DELETE);
        }
    }

    displaySavedGames(games) {
        const container = document.getElementById('savedGames');
        container.innerHTML = '';

        games.forEach(game => {
            const row = document.createElement('tr');
            
            const nameCell = document.createElement('td');
            nameCell.textContent = game.name;
            
            const dateCell = document.createElement('td');
            dateCell.textContent = new Date(game.saveDate).toLocaleString();
            
            const iterationCell = document.createElement('td');
            iterationCell.textContent = game.iteration;
            
            const animalCountsCell = document.createElement('td');
            const counts = [];
            if (game.animalCounts.lion > 0) {
                counts.push(`${game.animalCounts.lion} ${GameConstants.Animals.Icons.LION}`);
            }
            if (game.animalCounts.antelope > 0) {
                counts.push(`${game.animalCounts.antelope} ${GameConstants.Animals.Icons.ANTELOPE}`);
            }
            if (game.animalCounts.tiger > 0) {
                counts.push(`${game.animalCounts.tiger} ${GameConstants.Animals.Icons.TIGER}`);
            }
            if (game.animalCounts.zebra > 0) {
                counts.push(`${game.animalCounts.zebra} ${GameConstants.Animals.Icons.ZEBRA}`);
            }
            animalCountsCell.textContent = counts.join(' | ');
            
            const actionsCell = document.createElement('td');
            const loadButton = document.createElement('button');
            loadButton.className = 'btn btn-primary btn-sm me-2';
            loadButton.textContent = 'Load';
            loadButton.onclick = () => this.loadGame(game.id);
            
            const deleteButton = document.createElement('button');
            deleteButton.className = 'btn btn-danger btn-sm';
            deleteButton.textContent = 'Delete';
            deleteButton.onclick = () => this.deleteSave(game.id);
            
            actionsCell.appendChild(loadButton);
            actionsCell.appendChild(deleteButton);
            
            row.appendChild(nameCell);
            row.appendChild(dateCell);
            row.appendChild(iterationCell);
            row.appendChild(animalCountsCell);
            row.appendChild(actionsCell);
            
            container.appendChild(row);
        });
    }
}

// Export as global instance
window.saveManager = new SaveManager(); 