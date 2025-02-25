using Savanna.GameEngine.Constants;
using Savanna.Common.Models;
using Savanna.Common.Interfaces;
using System.Linq;
using System;

namespace Savanna.GameEngine.Models
{
    /// <summary>
    /// Handles reproduction logic specific to Lions
    /// </summary>
    public class LionReproductionManager : ReproductionManager
    {
        private readonly Lion _parent;

        /// <summary>
        /// Initializes a new reproduction manager for a Lion
        /// </summary>
        /// <param name="parent">The parent Lion this manager belongs to</param>
        /// <param name="healthManager">The health manager to check reproduction eligibility</param>
        public LionReproductionManager(Lion parent, IHealthManageable healthManager) 
            : base(healthManager)
        {
            _parent = parent;
        }

        /// <summary>
        /// Checks if there is a potential mate nearby within mating distance
        /// </summary>
        /// <returns>True if a potential mate is found within range</returns>
        protected override bool IsNearMate(IGameField field)
        {
            return field.GetEntitiesOfTypeInRange<Lion>(_parent.Position, (int)GameConstants.Reproduction.MatingDistance)
                .Where(l => l.IsAlive && l != _parent)
                .Any();
        }

        /// <summary>
        /// Creates a new Lion offspring at a slightly offset position from the parent
        /// </summary>
        /// <param name="position">The base position for the offspring</param>
        /// <returns>A new Lion instance</returns>
        public override IGameEntity Reproduce(Position position)
        {
            var newPosition = new Position(
                position.X + Random.Shared.Next(-1, 2),
                position.Y + Random.Shared.Next(-1, 2)
            );
            
            return new Lion(newPosition, GetParentConfiguration());
        }

        /// <summary>
        /// Gets the configuration from the parent Lion for offspring creation
        /// </summary>
        /// <returns>The parent's configuration</returns>
        /// <exception cref="InvalidOperationException">Thrown when parent configuration is not available</exception>
        private IAnimalConfiguration GetParentConfiguration()
        {
            var config = _parent.GetConfiguration();
            if (config == null)
            {
                throw new InvalidOperationException(GameConstants.ErrorMessages.ParentConfigurationNotAvailable);
            }
            return config;
        }
    }
} 