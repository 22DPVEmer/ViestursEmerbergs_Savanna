namespace Savanna.Services.Constants
{
    public static class GameConstants
    {
        public static class AnimalTypes
        {
            public const string Lion = "Lion";
            public const string Antelope = "Antelope";
            public const string Tiger = "Tiger";
            public const string Zebra = "Zebra";
        }

        public static class ConnectionStrings
        {
            public const string DefaultConnection = "DefaultConnection";
        }

        public static class Routes
        {
            public const string SavedGames = "saved";
            public const string LoadGame = "load/{saveId}";
            public const string GameState = "state";
            public const string AddAnimal = "add-animal";
            public const string AnimalDetails = "animal/{animalId}";
            public const string TogglePause = "toggle-pause";
            public const string AnimalTypes = "animal-types";
        }

        public static class QueryParameters
        {
            public const string SaveId = "saveId";
            public const string AnimalId = "animalId";
        }

        public static class DefaultValues
        {
            public const int BoardWidth = 20;
            public const int BoardHeight = 20;
            public const int DefaultHealth = 100;
        }
    }
} 