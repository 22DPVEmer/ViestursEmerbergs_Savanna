namespace Savanna.GameEngine.Constants
{
    /// <summary>
    /// Contains all game-related constants and configuration values
    /// </summary>
    public static class GameConstants
    {
        /// <summary>
        /// Constants related to the game field dimensions and display
        /// </summary>
        public static class Field
        {
            public const int DefaultWidth = 20;
            public const int DefaultHeight = 10;
            public const char EmptyCell = '.';
            public const int GameTickMs = 500;
        }

        /// <summary>
        /// Constants related to animal movement and direction calculations
        /// </summary>
        public static class Movement
        {
            public const int DirectionCount = 8;
            public const int DirectionBase = 3; // Used for calculating direction offsets
            public const int MinDirectionOffset = -1;
            public const int MaxDirectionOffset = 1;
        }

        /// <summary>
        /// Constants related to animal health and damage calculations
        /// </summary>
        public static class Health
        {
            public const double InitialHealth = 20.0;
            public const double MovementHealthCost = 0.5;
            public const double PreyHealthValue = 10.0;
            public const double MinimumHealth = 0.0;
            public const double DeathThreshold = 0.1;
        }

        /// <summary>
        /// Constants related to animal reproduction mechanics
        /// </summary>
        public static class Reproduction
        {
            public const int RequiredConsecutiveRounds = 3;
            public const double MatingDistance = 2.0;
            public const double MinimumHealthToReproduce = 10.0;
            public const double ReproductionHealthCost = 5.0;
        }

        /// <summary>
        /// Constants specific to different animal types
        /// </summary>
        public static class Animal
        {
            /// <summary>
            /// Constants specific to Antelope behavior and properties
            /// </summary>
            public static class Antelope
            {
                public const char Symbol = 'A';
                public const int Speed = 2;
                public const int VisionRange = 4;
            }

            /// <summary>
            /// Constants specific to Lion behavior and properties
            /// </summary>
            public static class Lion
            {
                public const char Symbol = 'L';
                public const int Speed = 1;
                public const int VisionRange = 5;
                public const double CatchDistance = 1.0;
            }
        }

        public static class UserInterface
        {
            public const string WelcomeMessage = "Welcome to Savanna Game!";
            public const string AddAntelopeInstruction = "Press 'A' to add Antelope";
            public const string AddLionInstruction = "Press 'L' to add Lion";
            public const string QuitInstruction = "Press 'Q' to quit";
        }
    }
} 