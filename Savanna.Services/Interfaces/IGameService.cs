using Savanna.Services.Game.Models;
using Savanna.Common.Interfaces;

namespace Savanna.Services.Interfaces;

public interface IGameService
{
    Task StartGame(string userId);
    Task QuitGame(string userId);
    Task SaveGame(string userId);
    Task LoadGame(int saveId, string userId);
    Task AddAnimal(string userId, string type);
    GameStateDto? GetGameState(string userId);
    AnimalDetailsDto? GetAnimalDetails(string userId, string animalId);
    Task<GameStateDto?> UpdateGameStateAsync(string userId);
    bool TogglePause(string userId);
    IEnumerable<string> GetAvailableAnimalTypes();
}

public record GameState(
    Guid Id,
    string UserId,
    int Iteration,
    Dictionary<string, int> AnimalCounts,
    IGameEntity[,] Field,
    DateTime LastUpdated,
    bool IsActive
);

public record GameSummary(
    Guid Id,
    int Iteration,
    Dictionary<string, int> AnimalCounts,
    DateTime LastSaved
); 