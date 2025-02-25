using Savanna.Common.Interfaces;

namespace Savanna.Services.Game.Models;

/// <summary>
/// Tracks additional state for animals in the web interface without modifying the game engine
/// </summary>
public class AnimalState
{
    public IGameEntity Entity { get; }
    public int Age { get; set; }
    public int OffspringCount { get; set; }

    public AnimalState(IGameEntity entity)
    {
        Entity = entity;
        Age = 0;
        OffspringCount = 0;
    }
} 