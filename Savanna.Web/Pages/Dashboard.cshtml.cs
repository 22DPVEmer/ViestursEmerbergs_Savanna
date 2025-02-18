using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Savanna.Infrastructure.Data;
using Savanna.Infrastructure.Models;

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

        public async Task OnGetAsync()
        {
            SavedGames = await _context.GameSaves
                .Where(g => g.UserId == User.Identity!.Name)
                .OrderByDescending(g => g.SaveDate)
                .ToListAsync();
        }
    }
} 