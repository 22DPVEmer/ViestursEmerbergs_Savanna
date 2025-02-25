using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Savanna.Infrastructure.Models;
using System;

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

            // Configure GameSave -> GameState one-to-one relationship
            builder.Entity<GameState>()
                .HasOne(g => g.GameSave)
                .WithOne(s => s.GameState)
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

            // Configure GameState table
            builder.Entity<GameState>()
                .ToTable("GameStates")
                .HasKey(g => g.Id);

            // Configure AnimalState table
            builder.Entity<AnimalState>()
                .ToTable("AnimalStates")
                .HasKey(a => a.Id);

            // Seed Users
            var hasher = new PasswordHasher<ApplicationUser>();
            
            var users = new[]
            {
                new ApplicationUser
                {
                    Id = "1",
                    UserName = "player1@savanna.com",
                    NormalizedUserName = "PLAYER1@SAVANNA.COM",
                    Email = "player1@savanna.com",
                    NormalizedEmail = "PLAYER1@SAVANNA.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new ApplicationUser
                {
                    Id = "2",
                    UserName = "player2@savanna.com",
                    NormalizedUserName = "PLAYER2@SAVANNA.COM",
                    Email = "player2@savanna.com",
                    NormalizedEmail = "PLAYER2@SAVANNA.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new ApplicationUser
                {
                    Id = "3",
                    UserName = "player3@savanna.com",
                    NormalizedUserName = "PLAYER3@SAVANNA.COM",
                    Email = "player3@savanna.com",
                    NormalizedEmail = "PLAYER3@SAVANNA.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new ApplicationUser
                {
                    Id = "4",
                    UserName = "player4@savanna.com",
                    NormalizedUserName = "PLAYER4@SAVANNA.COM",
                    Email = "player4@savanna.com",
                    NormalizedEmail = "PLAYER4@SAVANNA.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                },
                new ApplicationUser
                {
                    Id = "5",
                    UserName = "player5@savanna.com",
                    NormalizedUserName = "PLAYER5@SAVANNA.COM",
                    Email = "player5@savanna.com",
                    NormalizedEmail = "PLAYER5@SAVANNA.COM",
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                }
            };

            foreach (var user in users)
            {
                user.PasswordHash = hasher.HashPassword(user, "Test123!");
                builder.Entity<ApplicationUser>().HasData(user);
            }
        }
    }
} 