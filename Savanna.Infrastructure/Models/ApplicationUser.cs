using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Savanna.Infrastructure.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<GameSave> GameSaves { get; set; } = new List<GameSave>();
    }
} 