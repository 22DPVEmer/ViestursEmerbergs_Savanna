namespace Savanna.Common.Interfaces
{
    /// <summary>
    /// Defines health-related behaviors for game entities
    /// </summary>
    public interface IHealthManageable
    {
        double Health { get; }
        bool IsAlive { get; }
        void DecreaseHealth(double amount);
        void IncreaseHealth(double amount);
        void Die();
    }
} 