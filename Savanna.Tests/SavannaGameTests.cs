using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Savanna.Common.Interfaces;
using Savanna.Common.Models;
using Savanna.GameEngine;
using Savanna.GameEngine.Constants;
using Savanna.GameEngine.Models;

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

            public TestAnimalFactory(
                TestAnimalConfiguration lionConfig, 
                TestAnimalConfiguration antelopeConfig)
            {
                _lionConfig = lionConfig;
                _antelopeConfig = antelopeConfig;
            }

            public IGameEntity CreateAnimal(char type, Position position)
            {
                return type switch
                {
                    TestConstants.AnimalSymbols.Lion => new Lion(position, _lionConfig),
                    TestConstants.AnimalSymbols.Antelope => new Antelope(position, _antelopeConfig),
                    _ => throw new ArgumentException(string.Format(TestConstants.Messages.UnknownAnimalType, type))
                };
            }

            public IEnumerable<char> GetAvailableAnimalTypes()
            {
                return new[] { TestConstants.AnimalSymbols.Lion, TestConstants.AnimalSymbols.Antelope };
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
                Symbol = TestConstants.AnimalSymbols.Lion,
                Speed = 1,
                VisionRange = 5
            };

            _antelopeConfig = new TestAnimalConfiguration
            {
                Symbol = TestConstants.AnimalSymbols.Antelope,
                Speed = 2,
                VisionRange = 4
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
            var position = new Position(0, 0);
            var lion = _animalFactory.CreateAnimal(TestConstants.AnimalSymbols.Lion, position);
            Assert.AreEqual(GameConstants.Health.InitialHealth, lion.Health);
        }

        /// <summary>
        /// Verifies that animal health decreases by the correct amount when moving
        /// </summary>
        [TestMethod]
        public void Animal_Move_ShouldDecreaseHealth()
        {
            var position = new Position(0, 0);
            _field.AddAnimal(TestConstants.AnimalSymbols.Lion, position);
            var lion = _field.Animals.First(a => a.Symbol == TestConstants.AnimalSymbols.Lion);
            double initialHealth = lion.Health;
            _field.Update();
            Assert.AreEqual(initialHealth - GameConstants.Health.MovementHealthCost, lion.Health);
        }

        /// <summary>
        /// Verifies that a lion's health increases when catching an antelope, accounting for movement cost
        /// </summary>
        [TestMethod]
        public void Lion_CatchAntelope_ShouldIncreaseHealth()
        {
            var position = new Position(0, 0);
            _field.AddAnimal(TestConstants.AnimalSymbols.Lion, position);
            _field.AddAnimal(TestConstants.AnimalSymbols.Antelope, new Position(0, 1)); // Ensure antelope is within catch distance

            var lion = _field.Animals.First(a => a.Symbol == TestConstants.AnimalSymbols.Lion);
            var antelope = _field.Animals.First(a => a.Symbol == TestConstants.AnimalSymbols.Antelope);
            var healthLion = (IHealthManageable)lion;
            double initialHealth = healthLion.Health;

            _field.Update();

            // Assert that the lion's health is at expected value after movement cost
            Assert.AreEqual(19.5, healthLion.Health, TestConstants.Messages.HealthAfterMovementAndCatch);
            Assert.IsFalse(antelope.IsAlive, TestConstants.Messages.PreyShouldBeDead);
        }

        /// <summary>
        /// Verifies that animals die when their health drops below the death threshold
        /// </summary>
        [TestMethod]
        public void Animal_HealthBelowThreshold_ShouldDie()
        {
            var position = new Position(0, 0);
            _field.AddAnimal(TestConstants.AnimalSymbols.Lion, position);
            var lion = _field.Animals.First(a => a.Symbol == TestConstants.AnimalSymbols.Lion);
            var healthLion = (IHealthManageable)lion;

            // Directly reduce health below death threshold
            healthLion.DecreaseHealth(GameConstants.Health.InitialHealth - GameConstants.Health.DeathThreshold + 1);

            Assert.IsFalse(lion.IsAlive, TestConstants.Messages.DieWhenHealthBelowThreshold);
        }

        /// <summary>
        /// Verifies that animals can reproduce after being near each other for three consecutive rounds
        /// </summary>
        [TestMethod]
        public void Animals_NearForThreeRounds_ShouldReproduce()
        {
            // Create lions at adjacent positions
            var position1 = new Position(0, 0);
            var position2 = new Position(1, 0);
            _field.AddAnimal(TestConstants.AnimalSymbols.Lion, position1);
            _field.AddAnimal(TestConstants.AnimalSymbols.Lion, position2);

            // Ensure lions have enough health to reproduce
            var lion1 = (IHealthManageable)_field.Animals.First(a => a.Symbol == TestConstants.AnimalSymbols.Lion);
            var lion2 = (IHealthManageable)_field.Animals.Last(a => a.Symbol == TestConstants.AnimalSymbols.Lion);
            lion1.IncreaseHealth(GameConstants.Reproduction.MinimumHealthToReproduce * 2);
            lion2.IncreaseHealth(GameConstants.Reproduction.MinimumHealthToReproduce * 2);

            // Update for required consecutive rounds and verify positions
            for (int i = 0; i < GameConstants.Reproduction.RequiredConsecutiveRounds; i++)
            {
                _field.Update();
                // Verify lions stay within reproduction range
                var updatedLion1 = _field.Animals.First(a => a.Symbol == TestConstants.AnimalSymbols.Lion);
                var updatedLion2 = _field.Animals.Last(a => a.Symbol == TestConstants.AnimalSymbols.Lion);
                var distance = updatedLion1.Position.DistanceTo(updatedLion2.Position);
                Assert.IsTrue(distance <= GameConstants.Reproduction.MatingDistance, 
                    $"Lions must stay within mating distance ({GameConstants.Reproduction.MatingDistance}). Current distance: {distance}");
            }

            // Verify reproduction occurred
            Assert.AreEqual(3, _field.Animals.Count(a => a.Symbol == TestConstants.AnimalSymbols.Lion), TestConstants.Messages.AnimalsNearShouldReproduce);
        }

        /// <summary>
        /// Verifies that animals cannot reproduce when they are too far apart
        /// </summary>
        [TestMethod]
        public void Animals_TooFarApart_ShouldNotReproduce()
        {
            var position1 = new Position(0, 0);
            var position2 = new Position(5, 5);
            _field.AddAnimal(TestConstants.AnimalSymbols.Lion, position1);
            _field.AddAnimal(TestConstants.AnimalSymbols.Lion, position2);

            // Update for required consecutive rounds
            for (int i = 0; i < GameConstants.Reproduction.RequiredConsecutiveRounds; i++)
            {
                _field.Update();
            }

            // Verify that no new lion was created
            Assert.AreEqual(2, _field.Animals.Count);
        }

        /// <summary>
        /// Verifies that animals cannot reproduce when their health is below the minimum threshold
        /// </summary>
        [TestMethod]
        public void Animal_Reproduction_WithInsufficientHealth_ShouldNotWork()
        {
            var position1 = new Position(0, 0);
            var position2 = new Position(1, 0);
            _field.AddAnimal(TestConstants.AnimalSymbols.Lion, position1);
            _field.AddAnimal(TestConstants.AnimalSymbols.Lion, position2);

            // Reduce health below reproduction threshold for both lions
            var lion1 = (IHealthManageable)_field.Animals.First(a => a.Symbol == TestConstants.AnimalSymbols.Lion);
            var lion2 = (IHealthManageable)_field.Animals.Last(a => a.Symbol == TestConstants.AnimalSymbols.Lion);
            lion1.DecreaseHealth(lion1.Health - GameConstants.Reproduction.MinimumHealthToReproduce + 1);
            lion2.DecreaseHealth(lion2.Health - GameConstants.Reproduction.MinimumHealthToReproduce + 1);

            // Update for required consecutive rounds
            for (int i = 0; i < GameConstants.Reproduction.RequiredConsecutiveRounds; i++)
            {
                _field.Update();
            }

            // Verify current behavior (2 lions, no reproduction with low health)
            Assert.AreEqual(2, _field.Animals.Count(a => a.Symbol == TestConstants.AnimalSymbols.Lion), TestConstants.Messages.LionsNotReproduceWithLowHealth);
        }
    }
} 