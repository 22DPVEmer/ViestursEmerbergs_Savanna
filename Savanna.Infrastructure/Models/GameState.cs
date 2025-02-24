using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Savanna.Infrastructure.Models
{
    public class GameState
    {
        [Key]
        public int Id { get; set; }

        public int GameSaveId { get; set; }

        [ForeignKey("GameSaveId")]
        public virtual GameSave GameSave { get; set; }

        [Required]
        public int BoardWidth { get; set; }

        [Required]
        public int BoardHeight { get; set; }

        [Required]
        public int CurrentIteration { get; set; } = 0;  // Tracks the current game iteration

        public virtual ICollection<AnimalState> Animals { get; set; } = new List<AnimalState>();
    }
} 