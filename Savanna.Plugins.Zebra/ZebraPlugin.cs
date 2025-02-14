using System;
using Savanna.Common.Models;
using Savanna.Common.Interfaces;
using Savanna.Plugins.Zebra.Constants;

namespace Savanna.Plugins.Zebra
{
    /// <summary>
    /// Main plugin class for the Zebra animal.
    /// Implements IAnimalPlugin to integrate with the game.
    /// </summary>
    public class ZebraPlugin : IAnimalPlugin
    {
        private readonly ZebraConfiguration _configuration = new();

        public string AnimalName => "Zebra";
        public char Symbol => _configuration.Symbol;
        public IAnimalConfiguration Configuration => _configuration;
        public string Version => "1.0.0";
        public bool IsCompatible => true;

        public IGameEntity CreateAnimal(Position position)
        {
            return new Zebra(position, Configuration);
        }
    }
}