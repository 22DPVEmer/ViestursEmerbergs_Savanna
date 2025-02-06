using Savanna.ConsoleApp.Display;
using Savanna.ConsoleApp.Rendering;
using Savanna.GameEngine;
using Savanna.GameEngine.Constants;
using Savanna.GameEngine.Factory;
using Savanna.GameEngine.Models;

namespace Savanna.ConsoleApp.Game
{
    /// <summary>
    /// Handles the game loop and control logic
    /// </summary>
    public class GameRunner
    {
        private readonly GameField _gameField;
        private readonly IFieldRenderer _fieldRenderer;
        private readonly GameDisplay _display;
        private static readonly Random Random = new();

        public GameRunner()
        {
            // Create dependencies
            var animalFactory = new AnimalFactory();
            _fieldRenderer = new ConsoleFieldRenderer();
            _display = new GameDisplay();
            
            // Create game field with dependencies
            _gameField = new GameField(animalFactory);
        }

        /// <summary>
        /// Main game loop that handles the game flow
        /// </summary>
        public void Run()
        {
            // Show initial instructions to the user
            _display.DisplayInstructions();
            
            // Start the main game loop
            RunGameLoop();
        }

        /// <summary>
        /// Main game loop that continuously updates and displays the game state
        /// </summary>
        private void RunGameLoop()
        {
            bool isRunning = true;
            while (isRunning)
            {
                // Check for user input
                if (Console.KeyAvailable)
                {
                    isRunning = HandleUserInput();
                }

                // Update game state and display
                UpdateAndDisplayGame();
                
                // Wait for next frame
                Thread.Sleep(GameConstants.Field.GameTickMs);
            }
        }

        /// <summary>
        /// Handles user keyboard input
        /// Returns false if the game should end, true otherwise
        /// </summary>
        private bool HandleUserInput()
        {
            var key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.A: // Add Antelope
                    AddRandomAnimal(GameConstants.Animal.Antelope.Symbol);
                    return true;
                case ConsoleKey.L: // Add Lion
                    AddRandomAnimal(GameConstants.Animal.Lion.Symbol);
                    return true;
                case ConsoleKey.Q: // Quit game
                    return false;
                default:
                    return true;
            }
        }

        /// <summary>
        /// Adds a new animal at a random position on the field
        /// </summary>
        private void AddRandomAnimal(char type)
        {
            // Generate random position within field bounds
            var position = new Position(
                Random.Next(_gameField.Width),
                Random.Next(_gameField.Height)
            );
            _gameField.AddAnimal(type, position);
        }

        /// <summary>
        /// Updates the game state and refreshes the display
        /// </summary>
        private void UpdateAndDisplayGame()
        {
            _gameField.Update();
            var state = _fieldRenderer.RenderField(_gameField.Width, _gameField.Height, _gameField.Animals);
            _display.DisplayField(state, _gameField.Height, _gameField.Width);
        }
    }
} 