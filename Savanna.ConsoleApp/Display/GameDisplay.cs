using Savanna.GameEngine.Constants;

namespace Savanna.ConsoleApp.Display
{
    /// <summary>
    /// Handles all display-related functionality for the game
    /// </summary>
    public class GameDisplay
    {
        /// <summary>
        /// Displays the game instructions to the user
        /// </summary>
        public void DisplayInstructions()
        {
            Console.Clear();  // Clear screen before displaying instructions
            Console.WriteLine(GameConstants.UserInterface.WelcomeMessage);
            Console.WriteLine(GameConstants.UserInterface.AddAntelopeInstruction);
            Console.WriteLine(GameConstants.UserInterface.AddLionInstruction);
            Console.WriteLine(GameConstants.UserInterface.QuitInstruction);
            Console.WriteLine();
        }

        /// <summary>
        /// Displays the current state of the game field
        /// </summary>
        public void DisplayField(char[,] state, int height, int width)
        {
            // Move cursor to start of game field (below instructions)
            Console.SetCursorPosition(0, 4);
            
            // Display each cell of the field
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Console.Write(state[y, x]);
                }
                Console.WriteLine();
            }
        }
    }
} 