using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Savanna.Infrastructure.Models
{
    public class AnimalState
    {
        public int Id { get; set; }

        public int GameStateId { get; set; }

        [ForeignKey("GameStateId")]
        public GameState GameState { get; set; }

        [Required]
        public string AnimalType { get; set; }

        [Required]
        public int PositionX { get; set; }

        [Required]
        public int PositionY { get; set; }

        [Required]
        public int Health { get; set; }

        [Required]
        public bool IsAlive { get; set; } // I feel dead

        [Required]
        public int Age { get; set; } = 0;  // Tracks how many iterations the animal has been alive

        [Required]
        public int OffspringCount { get; set; } = 0;  // Tracks how many offspring this animal has produced

        [Required]
        public bool IsSelected { get; set; } = false;  // Tracks if the animal is currently selected in the UI

    }
} 