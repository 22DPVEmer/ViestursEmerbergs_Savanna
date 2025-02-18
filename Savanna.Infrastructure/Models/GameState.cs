using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Savanna.Infrastructure.Models
{
    public class GameState
    {
        public int Id { get; set; }

        public int GameSaveId { get; set; }

        [ForeignKey("GameSaveId")]
        public GameSave GameSave { get; set; }

        [Required]
        public int BoardWidth { get; set; }

        [Required]
        public int BoardHeight { get; set; }

        public ICollection<AnimalState> Animals { get; set; } = new List<AnimalState>();
    }
} 