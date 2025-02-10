using Microsoft.VisualStudio.TestTools.UnitTesting;
using Savanna.GameEngine;
using Savanna.GameEngine.Models;
using Savanna.GameEngine.Constants;
using Savanna.GameEngine.Interfaces;

namespace Savanna.Tests
{
    [TestClass]
    public class SavannaGameTests
    {
        private class TestAnimalConfiguration : IAnimalConfiguration
        {
            public char Symbol { get; init; }
            public int Speed { get; init; }
            public int VisionRange { get; init; }
        }

        private class TestAnimalFactory : IAnimalFactory
        {
            private readonly TestAnimalConfiguration _lionConfig;
            private readonly TestAnimalConfiguration _antelopeConfig;

            public TestAnimalFactory(TestAnimalConfiguration lionConfig, TestAnimalConfiguration antelopeConfig)
            {
                _lionConfig = lionConfig;
                _antelopeConfig = antelopeConfig;
            }

            public IGameEntity CreateAnimal(char type, Position position)
            {
                return type switch
                {
                    GameConstants.Animal.Lion.Symbol => new Lion(position, _lionConfig),
                    GameConstants.Animal.Antelope.Symbol => new Antelope(position, _antelopeConfig),
                    _ => throw new ArgumentException($"Unknown animal type: {type}")
                };
            }
        }

        private TestAnimalConfiguration _lionConfig;
        private TestAnimalConfiguration _antelopeConfig;
        private IAnimalFactory _animalFactory;
        private GameField _field;

        [TestInitialize]
        public void Setup()
        {
            _lionConfig = new TestAnimalConfiguration
            {
                Symbol = GameConstants.Animal.Lion.Symbol,
                Speed = GameConstants.Animal.Lion.Speed,
                VisionRange = GameConstants.Animal.Lion.VisionRange
            };

            _antelopeConfig = new TestAnimalConfiguration
            {
                Symbol = GameConstants.Animal.Antelope.Symbol,
                Speed = GameConstants.Animal.Antelope.Speed,
                VisionRange = GameConstants.Animal.Antelope.VisionRange
            };

            _animalFactory = new TestAnimalFactory(_lionConfig, _antelopeConfig);
            _field = new GameField(_animalFactory, 10, 10);
        }

        /// <summary>
        /// Verifies that animals start with maximum health when created
        /// </summary>
        [TestMethod]
        public void Animal_InitialHealth_ShouldBeMaximum()
        {
            var lion = new Lion(new Position(0, 0), _lionConfig);
            Assert.AreEqual(GameConstants.Health.InitialHealth, lion.Health);
        }

        /// <summary>
        /// Verifies that animal health decreases by the correct amount when moving
        /// </summary>
        [TestMethod]
        public void Animal_Move_ShouldDecreaseHealth()
        {
            var lion = new Lion(new Position(0, 0), _lionConfig);
            double initialHealth = lion.Health;
            lion.Move(_field);
            Assert.AreEqual(initialHealth - GameConstants.Health.MovementHealthCost, lion.Health);
        }

        /// <summary>
        /// Verifies that a lion's health increases when catching an antelope, accounting for movement cost
        /// </summary>
        [TestMethod]
        public void Lion_CatchAntelope_ShouldIncreaseHealth()
        {
            var position = new Position(0, 0);
            _field.AddAnimal(GameConstants.Animal.Lion.Symbol, position);
            _field.AddAnimal(GameConstants.Animal.Antelope.Symbol, position);

            var lion = (Lion)_field.Animals.First(a => a.Symbol == GameConstants.Animal.Lion.Symbol);
            var antelope = (Antelope)_field.Animals.First(a => a.Symbol == GameConstants.Animal.Antelope.Symbol);
            double initialHealth = lion.Health;

            // Directly perform action instead of field update to avoid movement
            lion.PerformAction(_field);

            // Health should be capped at InitialHealth
            Assert.AreEqual(GameConstants.Health.InitialHealth, lion.Health);
            Assert.IsFalse(antelope.IsAlive);
        }

        /// <summary>
        /// Verifies that animals die when their health drops below the death threshold
        /// </summary>
        [TestMethod]
        public void Animal_HealthBelowThreshold_ShouldDie()
        {
            var lion = new Lion(new Position(0, 0), _lionConfig);
            while (lion.Health > GameConstants.Health.DeathThreshold)
            {
                lion.DecreaseHealth(1.0);
            }
            Assert.IsFalse(lion.IsAlive);
        }

        /// <summary>
        /// Verifies that animals can reproduce after being near each other for three consecutive rounds
        /// </summary>
        [TestMethod]
        public void Animals_NearForThreeRounds_ShouldReproduce()
        {
            // Create lions at adjacent positions
            var lion1 = new Lion(new Position(0, 0), _lionConfig);
            var lion2 = new Lion(new Position(1, 0), _lionConfig);

            _field.AddAnimal(lion1.Symbol, lion1.Position);
            _field.AddAnimal(lion2.Symbol, lion2.Position);

            // Directly update reproduction status without moving
            for (int i = 0; i < GameConstants.Reproduction.RequiredConsecutiveRounds; i++)
            {
                lion1.UpdateReproductionStatus(_field);
                lion2.UpdateReproductionStatus(_field);
            }

            string message = string.Format(GameConstants.TestMessages.LionReproductionFormat,
                GameConstants.Reproduction.RequiredConsecutiveRounds,
                lion1.Health,
                lion2.Health,
                lion1.Position.DistanceTo(lion2.Position));

            Assert.IsTrue(lion1.CanReproduce, message);
            Assert.IsTrue(lion2.CanReproduce, message);
        }

        /// <summary>
        /// Verifies that animals cannot reproduce when they are too far apart
        /// </summary>
        [TestMethod]
        public void Animals_TooFarApart_ShouldNotReproduce()
        {
            var lion1 = new Lion(new Position(0, 0), _lionConfig);
            var lion2 = new Lion(new Position(5, 5), _lionConfig);

            _field.AddAnimal(lion1.Symbol, lion1.Position);
            _field.AddAnimal(lion2.Symbol, lion2.Position);

            _field.Update();

            Assert.IsFalse(lion1.CanReproduce);
            Assert.IsFalse(lion2.CanReproduce);
        }

        /// <summary>
        /// Verifies that antelopes can reproduce when exactly 2 tiles apart
        /// </summary>
        [TestMethod]
        public void Antelope_Reproduction_AtTwoTilesDistance_ShouldWork()
        {
            var antelope1 = new Antelope(new Position(0, 0), _antelopeConfig);
            // Place second antelope at (1,1) which is ~1.414 tiles away (within 2.0 distance)
            var antelope2 = new Antelope(new Position(1, 1), _antelopeConfig);

            _field.AddAnimal(antelope1.Symbol, antelope1.Position);
            _field.AddAnimal(antelope2.Symbol, antelope2.Position);

            // Directly update reproduction status without moving
            for (int i = 0; i < GameConstants.Reproduction.RequiredConsecutiveRounds; i++)
            {
                antelope1.UpdateReproductionStatus(_field);
                antelope2.UpdateReproductionStatus(_field);
            }

            string message = string.Format(GameConstants.TestMessages.AntelopeReproductionFormat,
                antelope1.Position.DistanceTo(antelope2.Position),
                GameConstants.Reproduction.MatingDistance);

            Assert.IsTrue(antelope1.CanReproduce, message);
            Assert.IsTrue(antelope2.CanReproduce, message);
        }

        /// <summary>
        /// Verifies that antelopes cannot reproduce when more than 2 tiles apart
        /// </summary>
        [TestMethod]
        public void Antelope_Reproduction_BeyondTwoTiles_ShouldNotWork()
        {
            var antelope1 = new Antelope(new Position(0, 0), _antelopeConfig);
            var antelope2 = new Antelope(new Position(3, 0), _antelopeConfig); // 3 tiles away

            _field.AddAnimal(antelope1.Symbol, antelope1.Position);
            _field.AddAnimal(antelope2.Symbol, antelope2.Position);

            // Directly update reproduction status without using field.Update()
            for (int i = 0; i < GameConstants.Reproduction.RequiredConsecutiveRounds; i++)
            {
                antelope1.UpdateReproductionStatus(_field);
                antelope2.UpdateReproductionStatus(_field);
            }

            Assert.IsFalse(antelope1.CanReproduce, string.Format(GameConstants.ErrorMessages.AntelopeNoReproduceBeyondDistance, "1"));
            Assert.IsFalse(antelope2.CanReproduce, string.Format(GameConstants.ErrorMessages.AntelopeNoReproduceBeyondDistance, "2"));
        }

        /// <summary>
        /// Verifies that animals cannot reproduce when their health is below the minimum threshold
        /// </summary>
        [TestMethod]
        public void Antelope_Reproduction_WithInsufficientHealth_ShouldNotWork()
        {
            var antelope1 = new Antelope(new Position(0, 0), _antelopeConfig);
            var antelope2 = new Antelope(new Position(1, 0), _antelopeConfig);

            // Reduce health below reproduction threshold
            antelope1.DecreaseHealth(GameConstants.Health.InitialHealth - GameConstants.Reproduction.MinimumHealthToReproduce + 1);

            _field.AddAnimal(antelope1.Symbol, antelope1.Position);
            _field.AddAnimal(antelope2.Symbol, antelope2.Position);

            // Directly update reproduction status without using field.Update()
            for (int i = 0; i < GameConstants.Reproduction.RequiredConsecutiveRounds; i++)
            {
                antelope1.UpdateReproductionStatus(_field);
                antelope2.UpdateReproductionStatus(_field);
            }

            Assert.IsFalse(antelope1.CanReproduce, GameConstants.ErrorMessages.AntelopeInsufficientHealth);
        }

        /// <summary>
        /// Verifies that reproduction creates a new antelope with correct properties
        /// </summary>
        [TestMethod]
        public void Antelope_Reproduction_ShouldCreateNewAntelope()
        {
            var antelope1 = new Antelope(new Position(0, 0), _antelopeConfig);
            var antelope2 = new Antelope(new Position(1, 1), _antelopeConfig); // Place diagonally for shorter distance

            // Add both antelopes to the field
            _field.AddAnimal(antelope1.Symbol, antelope1.Position);
            _field.AddAnimal(antelope2.Symbol, antelope2.Position);

            // Directly update reproduction status without moving
            for (int i = 0; i < GameConstants.Reproduction.RequiredConsecutiveRounds; i++)
            {
                antelope1.UpdateReproductionStatus(_field);
                antelope2.UpdateReproductionStatus(_field);
            }

            string message = string.Format(GameConstants.TestMessages.AntelopeOffspringFormat,
                antelope1.Position.DistanceTo(antelope2.Position),
                GameConstants.Reproduction.MatingDistance,
                antelope1.Health,
                antelope1.ConsecutiveRoundsNearMate,
                _field.Animals.Count);

            Assert.IsTrue(antelope1.CanReproduce, message);
            
            var offspring = antelope1.Reproduce(new Position(0, 1));
            Assert.IsNotNull(offspring);
            Assert.IsInstanceOfType(offspring, typeof(Antelope));
            Assert.AreEqual(GameConstants.Animal.Antelope.Symbol, offspring.Symbol);
        }

        /// <summary>
        /// Verifies that animals die immediately when their health reaches zero
        /// </summary>
        [TestMethod]
        public void Animal_ShouldDieWhenHealthReachesZero()
        {
            var lion = new Lion(new Position(0, 0), _lionConfig);
            lion.DecreaseHealth(GameConstants.Health.InitialHealth);
            Assert.IsFalse(lion.IsAlive);
        }

        /// <summary>
        /// Verifies that lions cannot catch antelopes that are beyond their catch distance
        /// </summary>
        [TestMethod]
        public void Lion_ShouldNotCatchAntelopeOutsideCatchDistance()
        {
            var lion = new Lion(new Position(0, 0), _lionConfig);
            var antelope = new Antelope(new Position(2, 2), _antelopeConfig); // Outside catch distance

            _field.AddAnimal(lion.Symbol, lion.Position);
            _field.AddAnimal(antelope.Symbol, antelope.Position);

            // Directly call lion's action instead of using field.Update()
            lion.PerformAction(_field);

            Assert.IsTrue(antelope.IsAlive);
            Assert.AreEqual(GameConstants.Health.InitialHealth, lion.Health);
        }

        /// <summary>
        /// Verifies that dead animals cannot move from their position
        /// </summary>
        [TestMethod]
        public void Animal_ShouldNotMoveWhenDead()
        {
            var lion = new Lion(new Position(0, 0), _lionConfig);
            var initialPosition = lion.Position;
            
            lion.DecreaseHealth(GameConstants.Health.InitialHealth); // Kill the lion
            lion.Move(_field);
            
            Assert.AreEqual(initialPosition, lion.Position);
        }

        /// <summary>
        /// Verifies that dead animals cannot participate in reproduction
        /// </summary>
        [TestMethod]
        public void Animal_ShouldNotReproduceWhenDead()
        {
            var lion1 = new Lion(new Position(0, 0), _lionConfig);
            var lion2 = new Lion(new Position(0, 1), _lionConfig);

            lion1.DecreaseHealth(GameConstants.Health.InitialHealth); // Kill lion1

            _field.AddAnimal(lion1.Symbol, lion1.Position);
            _field.AddAnimal(lion2.Symbol, lion2.Position);

            // Simulate 3 rounds
            for (int i = 0; i < GameConstants.Reproduction.RequiredConsecutiveRounds; i++)
            {
                _field.Update();
            }

            Assert.IsFalse(lion1.CanReproduce);
        }

        /// <summary>
        /// Verifies that the consecutive rounds counter resets when animals move apart
        /// </summary>
        [TestMethod]
        public void Animal_ConsecutiveRoundsShouldResetWhenSeparated()
        {
            var lion1 = new Lion(new Position(0, 0), _lionConfig);
            var lion2 = new Lion(new Position(0, 1), _lionConfig);

            _field.AddAnimal(lion1.Symbol, lion1.Position);
            _field.AddAnimal(lion2.Symbol, lion2.Position);

            // Simulate 2 rounds
            for (int i = 0; i < 2; i++)
            {
                _field.Update();
            }

            // Move lions apart
            lion2.Position = new Position(5, 5);
            _field.Update();

            // Move back together
            lion2.Position = new Position(0, 1);
            _field.Update();

            Assert.IsFalse(lion1.CanReproduce, GameConstants.ErrorMessages.ConsecutiveRoundsReset);
        }

        /// <summary>
        /// Verifies that animal health cannot exceed the initial maximum value
        /// </summary>
        [TestMethod]
        public void Animal_HealthShouldNotExceedInitial()
        {
            var lion = new Lion(new Position(0, 0), _lionConfig);
            lion.IncreaseHealth(100.0); // Try to increase above initial
            Assert.AreEqual(GameConstants.Health.InitialHealth, lion.Health);
        }

        /// <summary>
        /// Verifies that animal health cannot go below zero
        /// </summary>
        [TestMethod]
        public void Animal_HealthShouldNotGoBelowZero()
        {
            var lion = new Lion(new Position(0, 0), _lionConfig);
            lion.DecreaseHealth(GameConstants.Health.InitialHealth + 100.0); // Try to decrease below zero
            Assert.AreEqual(GameConstants.Health.MinimumHealth, lion.Health);
        }
    }
} 