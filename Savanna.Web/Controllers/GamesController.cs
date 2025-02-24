using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Savanna.Infrastructure.Data;
using Savanna.Services.Game.Models;
using Savanna.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Savanna.Services.Services;
using Savanna.Web.Constants;

namespace Savanna.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IGameService _gameService;
        private readonly ILogger<GamesController> _logger;

        public GamesController(ApplicationDbContext context, IGameService gameService, ILogger<GamesController> logger)
        {
            _context = context;
            _gameService = gameService;
            _logger = logger;
        }

        private string GetUserId()
        {
            // If authenticated, use the user's name
            if (User.Identity?.IsAuthenticated == true)
            {
                return User.Identity.Name!;
            }

            // For anonymous users, try to get or create a session ID
            var sessionId = HttpContext.Session.GetString("AnonymousUserId");
            if (string.IsNullOrEmpty(sessionId))
            {
                sessionId = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("AnonymousUserId", sessionId);
            }
            return sessionId;
        }

        [HttpGet("saved")]
        public async Task<IActionResult> GetSavedGames()
        {
            try
            {
                var userId = GetUserId();
                _logger.LogInformation(LoggerMessages.RetrievingSavedGames, userId);

                // Get the actual user ID if authenticated
                string actualUserId = userId;
                if (User.Identity?.IsAuthenticated == true)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userId);
                    if (user != null)
                    {
                        actualUserId = user.Id;
                    }
                }

                var games = await _context.GameSaves
                    .Include(g => g.GameState)
                    .ThenInclude(s => s.Animals)
                    .Where(g => g.UserId == actualUserId)
                    .Select(g => new
                    {
                        g.Id,
                        g.Name,
                        g.SaveDate,
                        Iteration = g.GameState.CurrentIteration,
                        AnimalCounts = new
                        {
                            Lion = g.GameState.Animals.Count(a => a.AnimalType == "Lion"),
                            Antelope = g.GameState.Animals.Count(a => a.AnimalType == "Antelope")
                        }
                    })
                    .OrderByDescending(g => g.SaveDate)
                    .ToListAsync();

                _logger.LogInformation(LoggerMessages.RetrievedSavedGames, games.Count, userId);
                return Ok(games);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LoggerMessages.ErrorRetrievingSavedGames, GetUserId());
                return StatusCode(500, new { message = ResponseMessages.ErrorRetrievingSavedGames + ex.Message });
            }
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartGame()
        {
            try
            {
                var userId = GetUserId();
                await _gameService.StartGame(userId);
                return Ok(new { message = ResponseMessages.GameStartedSuccess });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LoggerMessages.ErrorStartingGame);
                return StatusCode(500, new { message = ResponseMessages.ErrorStartingGame });
            }
        }

        [HttpPost("quit")]
        public async Task<IActionResult> QuitGame()
        {
            try
            {
                var userId = GetUserId();
                await _gameService.QuitGame(userId);
                return Ok(new { message = ResponseMessages.GameQuitSuccess });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LoggerMessages.ErrorQuittingGame);
                return StatusCode(500, new { message = ResponseMessages.ErrorQuittingGame });
            }
        }

        [HttpGet("state")]
        public IActionResult GetGameState()
        {
            try
            {
                var userId = GetUserId();
                var state = _gameService.GetGameState(userId);
                if (state == null)
                {
                    return NotFound(new { message = ResponseMessages.NoActiveGameFound });
                }
                return Ok(state);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LoggerMessages.ErrorRetrievingGameState);
                return StatusCode(500, new { message = ResponseMessages.ErrorRetrievingGameState });
            }
        }

        [HttpPost("save")]
        public async Task<IActionResult> SaveGame()
        {
            try
            {
                var userId = GetUserId();
                await _gameService.SaveGame(userId);
                return Ok(new { message = ResponseMessages.GameSavedSuccess });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LoggerMessages.ErrorSavingGame);
                return StatusCode(500, new { message = ResponseMessages.ErrorSavingGame });
            }
        }

        [HttpPost("load/{saveId}")]
        public async Task<IActionResult> LoadGame(int saveId)
        {
            try
            {
                var userId = GetUserId();
                await _gameService.LoadGame(saveId, userId);
                return Ok(new { message = ResponseMessages.GameLoadedSuccess });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LoggerMessages.ErrorLoadingGame);
                return StatusCode(500, new { message = ResponseMessages.ErrorLoadingGame });
            }
        }

        [HttpPost("add-animal")]
        public async Task<IActionResult> AddAnimal([FromBody] string type)
        {
            try
            {
                _logger.LogInformation(LoggerMessages.AddAnimalEndpointCalled, type);
                
                if (string.IsNullOrEmpty(type))
                {
                    _logger.LogWarning(LoggerMessages.InvalidAnimalType);
                    return BadRequest(new { message = ResponseMessages.AnimalTypeNullOrEmpty });
                }

                var userId = GetUserId();
                _logger.LogInformation(LoggerMessages.AddingAnimal, type, userId);
                
                await _gameService.AddAnimal(userId, type);
                
                _logger.LogInformation(LoggerMessages.SuccessfullyAddedAnimal, type, userId);
                return Ok(new { message = $"Successfully added {type}" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, LoggerMessages.OperationErrorAddingAnimal, ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, LoggerMessages.InvalidArgumentAddingAnimal, ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LoggerMessages.UnexpectedErrorAddingAnimal, type);
                return StatusCode(500, new { message = $"Failed to add animal: {ex.Message}" });
            }
        }

        [HttpGet("animal/{animalId}")]
        public IActionResult GetAnimalDetails(string animalId)
        {
            try
            {
                var userId = GetUserId();
                var details = _gameService.GetAnimalDetails(userId, animalId);
                if (details == null)
                {
                    return NotFound(new { message = ResponseMessages.AnimalNotFound });
                }
                return Ok(details);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LoggerMessages.ErrorRetrievingAnimalDetails);
                return StatusCode(500, new { message = ResponseMessages.ErrorRetrievingAnimalDetails });
            }
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateGameState()
        {
            try
            {
                var userId = GetUserId();
                var state = await _gameService.UpdateGameStateAsync(userId);
                if (state == null)
                {
                    return NotFound(new { message = ResponseMessages.NoActiveGameFound });
                }
                return Ok(state);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LoggerMessages.ErrorUpdatingGameState);
                return StatusCode(500, new { message = ResponseMessages.ErrorUpdatingGameState });
            }
        }

        [HttpPost("toggle-pause")]
        public IActionResult TogglePause()
        {
            try
            {
                var userId = GetUserId();
                var isPaused = _gameService.TogglePause(userId);
                return Ok(new { isPaused });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LoggerMessages.ErrorTogglingGamePause);
                return StatusCode(500, new { message = ResponseMessages.ErrorTogglingGamePause });
            }
        }

        [HttpGet("animal-types")]
        public IActionResult GetAnimalTypes()
        {
            try
            {
                var animalTypes = _gameService.GetAvailableAnimalTypes();
                return Ok(animalTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LoggerMessages.ErrorRetrievingAnimalTypes);
                return StatusCode(500, new { message = ResponseMessages.ErrorRetrievingAnimalTypes });
            }
        }

        [HttpDelete("saved/{saveId}")]
        public async Task<IActionResult> DeleteSave(int saveId)
        {
            try
            {
                var userId = GetUserId();
                
                // Find the save and its associated state
                var save = await _context.GameSaves
                    .Include(g => g.GameState)
                    .FirstOrDefaultAsync(g => g.Id == saveId && g.UserId == userId);

                if (save == null)
                {
                    return NotFound(new { message = ResponseMessages.SaveNotFound });
                }

                // Remove both the save and its state
                _context.GameStates.Remove(save.GameState);
                _context.GameSaves.Remove(save);
                
                await _context.SaveChangesAsync();
                
                return Ok(new { message = ResponseMessages.SaveDeletedSuccess });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, LoggerMessages.ErrorDeletingSave, saveId);
                return StatusCode(500, new { message = ResponseMessages.ErrorDeletingSave });
            }
        }
    }
} 