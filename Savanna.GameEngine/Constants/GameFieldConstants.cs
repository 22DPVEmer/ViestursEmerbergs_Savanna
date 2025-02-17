namespace Savanna.GameEngine.Constants
{
    public static class GameFieldConstants
    {
        public static class Messages
        {
            public const string UpdatingEntities = "\nUpdating {0} active entities";
            public const string ProcessingEntity = "Processing entity: {0} at {1} with symbol {2}";
            public const string EntityIsMovable = "Entity is movable, calling Move";
            public const string EntityNotMovable = "Entity is NOT movable";
            public const string EntityIsActionable = "Entity is actionable, calling PerformAction";
            public const string EntityNotActionable = "Entity is NOT actionable";
            public const string AfterMovePosition = "After move: position = {0}";
        }
    }
}