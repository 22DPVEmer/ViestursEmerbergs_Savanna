namespace Savanna.Plugins.Zebra.Constants
{
    /// <summary>
    /// Contains all Zebra-specific constants and messages
    /// </summary>
    public static class ZebraConstants
    {
        public static class Movement
        {
            public const int MovementCost = 1;
            public const int MaxMovementAttempts = 10;
        }

        public static class Reproduction
        {
            public const int RequiredConsecutiveRounds = 3;
            public const double MinimumHealthToReproduce = 10.0;
            public const int MatingDistance = 2;
            public const double ReproductionCost = 5.0;
        }
    }
}