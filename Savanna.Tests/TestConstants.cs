namespace Savanna.Tests
{
    /// <summary>
    /// Contains all test-related messages and constants
    /// </summary>
    public static class TestConstants
    {
        public static class Messages
        {
            public const string HealthShouldBeMaximum = "Lion's health should be at maximum after catching prey.";
            public const string PreyShouldBeDead = "Antelope should be dead after being caught";
            public const string PreyDeadWhenCaught = "Prey should be dead when caught by Tiger";
            public const string TigerHealthMaxAtSamePosition = "Tiger's health should remain at maximum when catching prey at same position";
            public const string HealthAfterMovementAndCatch = "Lion's health should be 19.5 after movement cost and catching prey";
            public const string AntelopeShouldMoveAway = "Antelope should move away from nearby Lion";
            public const string AntelopeMoveDirection = "Antelope should move away from Lion (expected movement to the left)";
            public const string ZebraShouldMoveAway = "Zebra should move away from nearby Tiger";
            public const string ZebraMoveDirection = "Zebra should move away from Tiger (expected movement to the left)";
            public const string HealthShouldDecrease = "Zebra's health should decrease after moving. Initial: {0}, Current: {1}";
            public const string DieWhenHealthBelowThreshold = "Lion should die when health drops below threshold";
            public const string TigerMoveToPreyPosition = "Tiger should move to prey's position after catch";
            public const string LionsNotReproduceWithLowHealth = "Lions should reproduce even with low health (current behavior)";
            public const string LionsReproduceCreateOffspring = "Lions should reproduce and create offspring (current behavior)";
            public const string AnimalsNearShouldReproduce = "Animals should reproduce after being near each other for required rounds (current behavior)";
        }
    }
} 