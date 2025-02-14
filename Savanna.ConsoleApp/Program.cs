using System;
using System.IO;
using System.Threading;
using System.Linq;
using Savanna.GameEngine;
using Savanna.GameEngine.Constants;
using Savanna.Common.Interfaces;
using Savanna.GameEngine.Factory;
using Savanna.ConsoleApp.Rendering;
using Savanna.ConsoleApp.Game;
using Savanna.Common.Models;
using Savanna.Common.Constants;

namespace Savanna.ConsoleApp
{
    /// <summary>
    /// Main game class that handles the user interface and game loop
    /// </summary>
    class Program
    {
        // Shared Random instance for generating random positions
        private static readonly Random Random = new();
        
        // The main game field that contains and manages all animals
        private readonly GameField _gameField;
        private readonly IFieldRenderer _fieldRenderer;
        private readonly IAnimalFactory _animalFactory;

        public Program()
        {
            // Set up plugins directory
            var pluginsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
            try
            {
                Directory.CreateDirectory(pluginsPath);
                Console.WriteLine(string.Format(PluginConstants.Messages.PluginDirectory, pluginsPath));
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(PluginConstants.Messages.PluginDirectoryError, ex.Message));
            }

            // Initialize game components
            _animalFactory = new AnimalFactory(pluginsPath);
            
            // Display available animal types
            Console.WriteLine(PluginConstants.Messages.AvailableAnimalsHeader);
            foreach (var type in _animalFactory.GetAvailableAnimalTypes())
            {
                Console.WriteLine(string.Format(PluginConstants.Messages.AnimalListItem, type));
            }
            Console.WriteLine(); // Empty line for spacing

            _gameField = new GameField(_animalFactory);
            _fieldRenderer = new ConsoleFieldRenderer();
        }

        /// <summary>
        /// Main entry point for the game
        /// </summary>
        static void Main(string[] args)
        {
            var program = new Program();
            program.Run();
        }

        /// <summary>
        /// Main game loop
        /// </summary>
        private void Run()
        {
            var gameRunner = new GameRunner(_gameField, _fieldRenderer);
            gameRunner.Run();
        }
    }
}