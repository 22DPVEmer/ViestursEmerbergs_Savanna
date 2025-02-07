using System;
using System.Threading;
using Savanna.GameEngine;
using Savanna.GameEngine.Constants;
using Savanna.GameEngine.Models;
using Savanna.GameEngine.Factory;
using Savanna.ConsoleApp.Rendering;
using Savanna.ConsoleApp.Game;

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

        public Program()
        {
            // Create dependencies
            var animalFactory = new AnimalFactory();
            _fieldRenderer = new ConsoleFieldRenderer();
            
            // Create game field with dependencies
            _gameField = new GameField(animalFactory);
        }

        /// <summary>
        /// Entry point of the application
        /// </summary>
        public static void Main(string[] args)
        {
            Console.CursorVisible = false;  // Hide cursor for better visualization
            
            // Create and run the game
            var game = new GameRunner();
            game.Run();
            
            Console.CursorVisible = true;  // Restore cursor visibility on exit
        }

        /// <summary>
        /// Main game loop that handles the game flow
        /// </summary>
        public void Run()
        {
            // Show initial instructions to the user
            DisplayInstructions();
            
            // Start the main game loop
            RunGameLoop();
        }

        /// <summary>
        /// Displays the game instructions to the user
        /// </summary>
        private void DisplayInstructions()
        {
            Console.Clear();  // Clear screen before displaying instructions
            Console.WriteLine(GameConstants.UserInterface.WelcomeMessage);
            Console.WriteLine(GameConstants.UserInterface.AddAntelopeInstruction);
            Console.WriteLine(GameConstants.UserInterface.AddLionInstruction);
            Console.WriteLine(GameConstants.UserInterface.QuitInstruction);
            Console.WriteLine();
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
            DisplayField();
        }

        /// <summary>
        /// Displays the current state of the game field
        /// </summary>
        private void DisplayField()
        {
            // Move cursor to start of game field (below instructions)
            Console.SetCursorPosition(0, 4);
            
            // Get current field state using the renderer
            var state = _fieldRenderer.RenderField(_gameField.Width, _gameField.Height, _gameField.Animals);

            // Display each cell of the field
            for (int y = 0; y < _gameField.Height; y++)
            {
                for (int x = 0; x < _gameField.Width; x++)
                {
                    Console.Write(state[y, x]);
                }
                Console.WriteLine();
            }
        }
    }
} 