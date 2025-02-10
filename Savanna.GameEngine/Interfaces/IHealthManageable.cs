namespace Savanna.GameEngine.Interfaces
{
    /// <summary>
    /// Defines health-related behaviors for game entities
    /// </summary>
    public interface IHealthManageable
    {
        // in a seperate file I have an isalive field that is used to check if the entity is dead
        double Health { get; }
        bool IsAlive { get; }
        void DecreaseHealth(double amount);
        void IncreaseHealth(double amount);
        void Die();
    }
} 