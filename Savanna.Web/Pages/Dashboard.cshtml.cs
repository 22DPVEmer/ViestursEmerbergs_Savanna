using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Savanna.Infrastructure.Data;
using Savanna.Infrastructure.Models;
using Savanna.Infrastructure.Constants;
using System.Linq;

namespace Savanna.Web.Pages
{
    [Authorize]
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DashboardModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<GameSave> SavedGames { get; set; } = new List<GameSave>();
        public int TotalGamesPlayed { get; set; }
        public int TotalAnimals { get; set; }
        public int LongestGameIterations { get; set; }

        public async Task OnGetAsync()
        {
            // Load saved games with their game states and animals
            SavedGames = await _context.GameSaves
                .Include(g => g.GameState)
                    .ThenInclude(s => s.Animals)
                .Where(g => g.UserId == User.Identity!.Name)
                .OrderByDescending(g => g.SaveDate)
                .ToListAsync();

            // Calculate statistics
            TotalGamesPlayed = await _context.GameSaves
                .Where(g => g.UserId == User.Identity!.Name)
                .CountAsync();

            TotalAnimals = await _context.GameSaves
                .Where(g => g.UserId == User.Identity!.Name)
                .SelectMany(g => g.GameState.Animals)
                .CountAsync();

            var longestGame = await _context.GameSaves
                .Where(g => g.UserId == User.Identity!.Name)
                .OrderByDescending(g => g.GameState.Animals.Count)
                .FirstOrDefaultAsync();

            LongestGameIterations = longestGame?.GameState?.Animals.Count ?? 0;
        }
    }

    public static class GameSaveExtensions
    {
        public static int GetCurrentIteration(this GameSave gameSave)
        {
            return gameSave.GameState?.Animals.Count ?? 0;
        }

        public static int GetLionCount(this GameSave gameSave)
        {
            return gameSave.GameState?.Animals.Count(a => a.AnimalType == AnimalTypes.Lion) ?? 0;
        }

        public static int GetAntelopeCount(this GameSave gameSave)
        {
            return gameSave.GameState?.Animals.Count(a => a.AnimalType == AnimalTypes.Antelope) ?? 0;
        }
    }
} 