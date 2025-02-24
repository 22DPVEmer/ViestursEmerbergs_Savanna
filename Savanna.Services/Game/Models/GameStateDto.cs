namespace Savanna.Services.Game.Models
{
    public class GameStateDto
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int Iteration { get; set; }
        public Dictionary<string, int> AnimalCounts { get; set; } = new();
        public List<AnimalDto> Animals { get; set; } = new();
        public bool IsPaused { get; set; }
    }

    public class AnimalDto
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int X { get; set; }
        public int Y { get; set; }
        public int Health { get; set; }
        public bool IsAlive { get; set; }
        public bool IsSelected { get; set; }
    }

    public class AnimalDetailsDto : AnimalDto
    {
        public int Age { get; set; }
        public int OffspringCount { get; set; }
    }
} 