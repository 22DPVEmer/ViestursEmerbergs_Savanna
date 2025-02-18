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
        public bool IsAlive { get; set; }
    }
} 