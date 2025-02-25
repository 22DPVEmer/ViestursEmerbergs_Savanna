using System.Text.Json.Serialization;

namespace Savanna.Services.Models
{
    public class AnimalConfig
    {
        public Configuration Configuration { get; set; } = new();
        public Movement Movement { get; set; } = new();
        public Reproduction Reproduction { get; set; } = new();
        public Hunting? Hunting { get; set; }
        public Plugin Plugin { get; set; } = new();
    }
} 