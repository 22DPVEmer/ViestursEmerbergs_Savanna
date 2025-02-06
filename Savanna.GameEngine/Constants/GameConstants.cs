namespace Savanna.GameEngine.Constants
{
    public static class GameConstants
    {
        public static class Field
        {
            public const int DefaultWidth = 20;
            public const int DefaultHeight = 10;
            public const char EmptyCell = '.';
            public const int GameTickMs = 500;
        }

        public static class Movement
        {
            public const int DirectionCount = 8;
            public const int DirectionBase = 3; // Used for calculating direction offsets
            public const int MinDirectionOffset = -1;
            public const int MaxDirectionOffset = 1;
        }

        public static class Animal
        {
            public static class Antelope
            {
                public const char Symbol = 'A';
                public const int Speed = 2;
                public const int VisionRange = 4;
            }

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