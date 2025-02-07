using Savanna.GameEngine.Constants;
using Savanna.GameEngine.Interfaces;
using Savanna.GameEngine.Models;

namespace Savanna.GameEngine.Factory
{
    /// <summary>
    /// Factory for creating animals with proper configuration.
    /// Centralizes animal creation and configuration.
    /// </summary>
    public class AnimalFactory : IAnimalFactory
    {
        private readonly Dictionary<char, IAnimalConfiguration> _configurations;

        public AnimalFactory()
        {
            _configurations = new Dictionary<char, IAnimalConfiguration>
            {
                { GameConstants.Animal.Antelope.Symbol, new AnimalConfiguration(
                    GameConstants.Animal.Antelope.Speed,
                    GameConstants.Animal.Antelope.VisionRange,
                    GameConstants.Animal.Antelope.Symbol)
                },
                { GameConstants.Animal.Lion.Symbol, new AnimalConfiguration(
                    GameConstants.Animal.Lion.Speed,
                    GameConstants.Animal.Lion.VisionRange,
                    GameConstants.Animal.Lion.Symbol)
                }
            };
        }

        public IGameEntity CreateAnimal(char type, Position position)
        {
            if (!_configurations.ContainsKey(type))
            {
                throw new ArgumentException($"Unknown animal type: {type}");
            }

            return type switch
            {
                var t when t == GameConstants.Animal.Antelope.Symbol => 
                    new Antelope(position, _configurations[t]),
                var t when t == GameConstants.Animal.Lion.Symbol => 
                    new Lion(position, _configurations[t]),
                _ => throw new ArgumentException($"Unhandled animal type: {type}")
            };
        }
    }

    /// <summary>
    /// Configuration record for animal properties.
    /// Implements IAnimalConfiguration for use in factory.
    /// </summary>
    internal record AnimalConfiguration(int Speed, int VisionRange, char Symbol) : IAnimalConfiguration;
} 