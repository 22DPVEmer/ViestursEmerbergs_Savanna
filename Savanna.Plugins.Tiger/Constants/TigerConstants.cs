namespace Savanna.Plugins.Tiger.Constants
{
    /// <summary>
    /// Contains all Tiger-specific constants and messages
    /// </summary>
    public static class TigerConstants
    {
        public static class Configuration
        {
            public const char Symbol = 'T';
            public const int Speed = 2;
            public const int VisionRange = 7;
        }

        public static class Movement
        {
            public const int MovementCost = 1;
            public const int MaxMovementAttempts = 10;
        }

        public static class Action
        {
            public const double CatchDistance = 1.5;
            public const double PreyHealthValue = 5.0;
        }

        public static class Reproduction
        {
            public const int RequiredConsecutiveRounds = 3;
            public const double MinimumHealthToReproduce = 10.0;
            public const int MatingDistance = 2;
            public const double ReproductionCost = 5.0;
        }

        public static class Symbols
        {
            public const char LionSymbol = 'L';  // Lions are too dangerous to attack
            public const char ZebraSymbol = 'Z';  // Zebras are prey
            public const char AntelopeSymbol = 'A';  // Antelopes are prey
        }

        public static class Plugin
        {
            public const string Name = "Tiger";
            public const string Version = "1.0.0";
        }
    }
}