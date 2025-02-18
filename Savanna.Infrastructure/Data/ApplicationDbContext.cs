using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Savanna.Infrastructure.Models;

namespace Savanna.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<GameSave> GameSaves { get; set; }
        public DbSet<GameState> GameStates { get; set; }
        public DbSet<AnimalState> AnimalStates { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure GameSave -> GameState relationship
            builder.Entity<GameSave>()
                .HasOne(g => g.GameState)
                .WithOne(s => s.GameSave)
                .HasForeignKey<GameState>(s => s.GameSaveId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure GameState -> AnimalState relationship
            builder.Entity<GameState>()
                .HasMany(g => g.Animals)
                .WithOne(a => a.GameState)
                .HasForeignKey(a => a.GameStateId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure User -> GameSave relationship
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.GameSaves)
                .WithOne(g => g.User)
                .HasForeignKey(g => g.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 