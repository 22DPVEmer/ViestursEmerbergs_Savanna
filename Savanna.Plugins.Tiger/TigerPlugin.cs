using System;
using Savanna.Common.Models;
using Savanna.Common.Interfaces;
using Savanna.Plugins.Tiger.Constants;

namespace Savanna.Plugins.Tiger
{
    /// <summary>
    /// Main plugin class for the Tiger animal.
    /// Implements IAnimalPlugin to integrate with the game.
    /// </summary>
    public class TigerPlugin : IAnimalPlugin
    {
        private readonly TigerConfiguration _configuration = new();

        public string AnimalName => "Tiger";
        public char Symbol => _configuration.Symbol;
        public IAnimalConfiguration Configuration => _configuration;
        public string Version => "1.0.0";
        public bool IsCompatible => true;

        public IGameEntity CreateAnimal(Position position)
        {
            return new Tiger(position, Configuration);
        }
    }
}