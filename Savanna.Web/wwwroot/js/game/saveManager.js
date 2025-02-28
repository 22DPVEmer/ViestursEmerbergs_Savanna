// Save Management
class SaveManager {
    constructor() {
        this.currentFilter = 'all';
        this.savedGames = [];
    }

    async loadSavedGames() {
        try {
            const response = await fetch(GameConstants.Api.Endpoints.SAVED);
            if (!response.ok) throw new Error('Failed to load saved games');
            
            this.savedGames = await response.json();
            this.renderSavedGames();
            this.initializeFilters();
        } catch (error) {
            console.error('Error loading saved games:', error);
            uiManager.showErrorMessage('Failed to load saved games');
        }
    }

    renderSavedGames(filteredGames = null) {
        const container = document.getElementById('savedGames');
        if (!container) return;

        const gamesToRender = filteredGames || this.filterGames(this.savedGames);
        container.innerHTML = ''; // Clear existing saves

        gamesToRender.forEach(game => {
            const gameItem = document.createElement('div');
            gameItem.className = 'saved-game-item';
            
            // Format the date for the game name
            const saveDate = new Date(game.saveDate);
            const formattedDate = saveDate.toLocaleString();
            const searchableDate = saveDate.toISOString();
            
            // Create header with date
            const header = document.createElement('div');
            header.className = 'saved-game-header';
            
            const title = document.createElement('h6');
            title.className = 'saved-game-title';
            title.textContent = formattedDate;
            title.dataset.searchDate = searchableDate;
            
            header.appendChild(title);
            
            // Create stats section
            const stats = document.createElement('div');
            stats.className = 'saved-game-stats';
            stats.innerHTML = `
                <span>Iteration: ${game.iteration}</span>
                <span>•</span>
                <span>Lions: ${game.animalCounts.lion || 0}</span>
                <span>•</span>
                <span>Antelopes: ${game.animalCounts.antelope || 0}</span>
                <span>•</span>
                <span>Tigers: ${game.animalCounts.tiger || 0}</span>
                <span>•</span>
                <span>Zebras: ${game.animalCounts.zebra || 0}</span>
            `;
            
            // Create controls section
            const controls = document.createElement('div');
            controls.className = 'saved-game-controls';
            controls.innerHTML = `
                <button class="btn btn-outline-primary" onclick="saveManager.loadGame(${game.id}, ${game.iteration})">
                    <i class="bi bi-play-fill"></i> Load
                </button>
                <button class="btn btn-outline-danger" onclick="saveManager.deleteSave(${game.id})">
                    <i class="bi bi-trash"></i>
                </button>
            `;
            
            // Assemble all sections
            gameItem.appendChild(header);
            gameItem.appendChild(stats);
            gameItem.appendChild(controls);
            
            container.appendChild(gameItem);
        });
    }

    filterGames(games) {
        const now = new Date();
        const today = new Date(now.getFullYear(), now.getMonth(), now.getDate());
        const weekAgo = new Date(today.getTime() - 7 * 24 * 60 * 60 * 1000);
        const monthAgo = new Date(today.getFullYear(), today.getMonth() - 1, today.getDate());

        switch (this.currentFilter) {
            case 'today':
                return games.filter(game => new Date(game.saveDate) >= today);
            case 'week':
                return games.filter(game => new Date(game.saveDate) >= weekAgo);
            case 'month':
                return games.filter(game => new Date(game.saveDate) >= monthAgo);
            default:
                return games;
        }
    }

    initializeFilters() {
        const filterButtons = document.querySelectorAll('#filterButtons .btn');
        filterButtons.forEach(button => {
            button.addEventListener('click', () => {
                filterButtons.forEach(btn => btn.classList.remove('active'));
                button.classList.add('active');
                this.currentFilter = button.dataset.filter;
                this.renderSavedGames();
            });
        });

        this.initializeSearch();
    }

    initializeSearch() {
        const searchInput = document.getElementById('savedGamesSearch');
        const searchContainer = searchInput?.parentElement;
        
        if (searchInput && searchContainer) {
            // Remove any existing search buttons first
            const existingButtons = searchContainer.querySelectorAll('button');
            existingButtons.forEach(button => button.remove());

            // Create search button
            const searchButton = document.createElement('button');
            searchButton.className = 'btn btn-outline-primary input-group-text';
            searchButton.innerHTML = '<i class="bi bi-search"></i>';
            searchContainer.appendChild(searchButton);

            // Function to perform search
            const performSearch = async () => {
                const searchTerm = searchInput.value.toLowerCase();
                try {
                    // Update URL with search term
                    const url = new URL(window.location);
                    if (searchTerm) {
                        url.searchParams.set('search', searchTerm);
                    } else {
                        url.searchParams.delete('search');
                    }
                    window.history.pushState({}, '', url);

                    if (!searchTerm) {
                        // If search is empty, just load all saved games
                        await this.loadSavedGames();
                        return;
                    }

                    // Make API call to search endpoint
                    const response = await fetch(`${GameConstants.Api.Endpoints.SAVED}/search?term=${encodeURIComponent(searchTerm)}`);
                    if (!response.ok) throw new Error('Failed to search saved games');
                    
                    const searchResults = await response.json();
                    this.renderSavedGames(searchResults);
                } catch (error) {
                    console.error('Error searching saved games:', error);
                    uiManager.showErrorMessage('Failed to search saved games');
                }
            };

            // Add click event to search button
            searchButton.addEventListener('click', performSearch);

            // Add enter key press event to search input
            searchInput.addEventListener('keypress', (e) => {
                if (e.key === 'Enter') {
                    performSearch();
                }
            });

            // Check for search term in URL on page load
            const url = new URL(window.location);
            const searchTerm = url.searchParams.get('search');
            if (searchTerm) {
                searchInput.value = searchTerm;
                performSearch();
            }
        }
    }

    async loadGame(saveId, savedIteration) {
        try {
            console.log(GameConstants.UI.Messages.Console.LOADING_GAME(saveId, savedIteration));
            
            // First, quit any existing game
            if (gameState.gameActive) {
                await gameControls.quitGame();
            }
            
            const response = await fetch(GameConstants.Api.Endpoints.LOAD_SAVE(saveId), {
                method: GameConstants.Api.Methods.POST
            });
            
            if (!response.ok) {
                const data = await response.json();
                throw new Error(data.message || GameConstants.UI.Messages.Error.FAILED_TO_VERIFY_STATE);
            }
            
            // Wait for game to initialize with retries
            let retries = 5;
            let stateData = null;
            let lastError = null;
            
            while (retries > 0 && !stateData) {
                try {
                    await new Promise(resolve => setTimeout(resolve, 1000));
                    console.log(GameConstants.UI.Messages.Console.VERIFYING_STATE(retries));
                    
                    const stateResponse = await fetch(GameConstants.Api.Endpoints.STATE);
                    console.log(GameConstants.UI.Messages.Console.RESPONSE_STATUS(stateResponse.status));
                    
                    if (stateResponse.ok) {
                        stateData = await stateResponse.json();
                        if (stateData) {
                            console.log(GameConstants.UI.Messages.Console.STATE_VERIFIED(stateData));
                            break;
                        }
                    } else {
                        lastError = GameConstants.UI.Messages.Error.STATE_VERIFICATION_FAILED(stateResponse.status);
                    }
                    
                    retries--;
                } catch (error) {
                    console.warn(GameConstants.UI.Messages.Console.STATE_VERIFY_FAILED(error));
                    lastError = error.message;
                    retries--;
                }
            }
            
            if (!stateData) {
                throw new Error(lastError || GameConstants.UI.Messages.Error.FAILED_TO_VERIFY_STATE);
            }

            gameState.gameActive = true;
            uiManager.enableGameControls();
            gameState.startGameStatePolling();
            uiManager.updateUI(stateData);
            uiManager.showSuccessMessage(GameConstants.UI.Messages.Success.GAME_LOADED);
        } catch (error) {
            console.error('Error loading game:', error);
            uiManager.showErrorMessage(GameConstants.UI.Messages.Error.SAVE_LOAD_ERROR(error.message));
            gameState.resetGameState();
        }
    }

    async saveGame() {
        try {
            const response = await fetch(GameConstants.Api.Endpoints.SAVE, { 
                method: GameConstants.Api.Methods.POST 
            });
            
            if (!response.ok) {
                const data = await response.json();
                throw new Error(data.message || GameConstants.UI.Messages.Error.FAILED_TO_SAVE);
            }
            
            const data = await response.json();
            console.log(GameConstants.UI.Messages.Console.SAVE_RESPONSE(data));
            
            gameState.hasUnsavedChanges = false;
            await this.loadSavedGames(); // Refresh the saves list
            uiManager.showSuccessMessage(GameConstants.UI.Messages.Success.GAME_SAVED);
        } catch (error) {
            console.error('Error saving game:', error);
            uiManager.showErrorMessage(GameConstants.UI.Messages.Error.FAILED_TO_SAVE);
        }
    }

    async deleteSave(saveId) {
        if (!confirm(GameConstants.UI.Messages.Confirm.DELETE_SAVE)) {
            return;
        }
        
        try {
            const response = await fetch(GameConstants.Api.Endpoints.DELETE_SAVE(saveId), { 
                method: GameConstants.Api.Methods.DELETE 
            });
            
            if (!response.ok) {
                const data = await response.json();
                throw new Error(data.message || GameConstants.UI.Messages.Error.FAILED_TO_DELETE_SAVE);
            }
            
            await this.loadSavedGames(); // Refresh the list
            uiManager.showSuccessMessage(GameConstants.UI.Messages.Success.SAVE_DELETED);
        } catch (error) {
            console.error('Error deleting save:', error);
            uiManager.showErrorMessage(GameConstants.UI.Messages.Error.SAVE_DELETE_ERROR(error.message));
        }
    }
}

// Export as global instance
window.saveManager = new SaveManager(); 