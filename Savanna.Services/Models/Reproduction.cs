namespace Savanna.Services.Models
{
    public class Reproduction
    {
        public int RequiredConsecutiveRounds { get; set; }
        public double MinimumHealthToReproduce { get; set; }
        public int MatingDistance { get; set; }
        public double ReproductionCost { get; set; }
    }
} 