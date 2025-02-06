using Savanna.GameEngine.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using Savanna.GameEngine.Models;
using Savanna.GameEngine.Interfaces;
using Savanna.GameEngine.Factory;

namespace Savanna.GameEngine
{
    /// <summary>
    /// Represents the game field where animals move and interact.
    /// Manages the collection of animals and their state updates.
    /// Acts as the main coordinator for the game's logic.
    /// </summary>
    public class GameField
    {
        /// <summary>
        /// Private list of animals for internal management.
        /// Using List for efficient add/remove operations.
        /// </summary>
        private readonly List<Animal> _animals;
        private readonly IAnimalFactory _animalFactory;

        /// <summary>
        /// Width of the game field in cells.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Height of the game field in cells.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Public read-only access to animals list.
        /// Returns AsReadOnly to prevent external modifications.
        /// </summary>
        public IReadOnlyList<Animal> Animals => _animals.AsReadOnly();

        /// <summary>
        /// Creates a new game field with specified or default dimensions.
        /// Uses constants for consistent game setup.
        /// </summary>
        public GameField(
            IAnimalFactory animalFactory,
            int width = GameConstants.Field.DefaultWidth,
            int height = GameConstants.Field.DefaultHeight)
        {
            Width = width;
            Height = height;
            _animals = new List<Animal>();
            _animalFactory = animalFactory ?? throw new ArgumentNullException(nameof(animalFactory));
        }

        /// <summary>
        /// Adds a new animal to the field based on type.
        /// Factory method pattern for creating animals.
        /// </summary>
        public void AddAnimal(char type, Position position)
        {
            var animal = _animalFactory.CreateAnimal(type, position);
            _animals.Add((Animal)animal);
        }

        /// <summary>
        /// Main update method called each game tick.
        /// Follows the Command pattern for update sequence:
        /// 1. Get list of active (living) animals
        /// 2. Update their positions and actions
        /// 3. Remove any dead animals
        /// </summary>
        public void Update()
        {
            var activeAnimals = GetActiveAnimals();
            UpdateAnimals(activeAnimals);
            RemoveDeadAnimals();
        }

        /// <summary>
        /// Gets list of all living animals.
        /// Uses LINQ for efficient filtering.
        /// </summary>
        private List<Animal> GetActiveAnimals() =>
            _animals.Where(a => a.IsAlive).ToList();

        /// <summary>
        /// Updates all active animals in sequence.
        /// Each animal:
        /// 1. Moves based on its behavior
        /// 2. Performs any special actions
        /// </summary>
        private void UpdateAnimals(List<Animal> activeAnimals)
        {
            foreach (var animal in activeAnimals)
            {
                animal.Move(this);
                animal.PerformAction(this);
            }
        }

        /// <summary>
        /// Removes dead animals from the field.
        /// Called after updates to clean up caught Antelopes.
        /// </summary>
        private void RemoveDeadAnimals() =>
            _animals.RemoveAll(a => !a.IsAlive);
    }
} 