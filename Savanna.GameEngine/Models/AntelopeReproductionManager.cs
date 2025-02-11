using Savanna.GameEngine.Constants;
using Savanna.GameEngine.Interfaces;
using System.Linq;
using System;

namespace Savanna.GameEngine.Models
{
    /// <summary>
    /// Handles reproduction logic specific to Antelopes.
    /// Implements mating distance checks and offspring creation.
    /// </summary>
    public class AntelopeReproductionManager : ReproductionManager
    {
        private readonly Animal _parent;

        /// <summary>
        /// Initializes a new instance of AntelopeReproductionManager.
        /// </summary>
        /// <param name="parent">The parent Antelope this manager belongs to</param>
        /// <param name="healthManager">The health manager to check reproduction eligibility</param>
        public AntelopeReproductionManager(Animal parent, IHealthManageable healthManager) 
            : base(healthManager)
        {
            _parent = parent;
        }

        /// <summary>
        /// Checks if there is a potential mate nearby.
        /// Looks for other living Antelopes within the mating distance.
        /// </summary>
        /// <returns>True if a potential mate is found within range</returns>
        protected override bool IsNearMate(GameField field)
        {
            return field.Animals
                .OfType<Antelope>()
                .Where(a => a.IsAlive && a != _parent)
                .Any(a => a.Position.DistanceTo(_parent.Position) <= GameConstants.Reproduction.MatingDistance);
        }

        /// <summary>
        /// Creates a new Antelope offspring at a position near the parent.
        /// The new position is randomly offset from the parent's position.
        /// </summary>
        /// <param name="position">The base position for the offspring</param>
        /// <returns>A new Antelope instance</returns>
        public override IGameEntity Reproduce(Position position)
        {
            // Create a new Antelope at a position near the parent
            var newPosition = new Position(
                position.X + Random.Shared.Next(-1, 2),
                position.Y + Random.Shared.Next(-1, 2)
            );
            
            return new Antelope(newPosition, GetParentConfiguration());
        }

        /// <summary>
        /// Gets the configuration from the parent Antelope.
        /// Throws if the parent configuration is not available.
        /// </summary>
        private IAnimalConfiguration GetParentConfiguration()
        {
            var config = (_parent as Antelope)?.GetConfiguration();
            if (config == null)
            {
                throw new InvalidOperationException(GameConstants.ErrorMessages.ParentConfigurationNotAvailable);
            }
            return config;
        }
    }
} 