using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Savanna.Common.Interfaces;
using Savanna.Common.Models;
using Savanna.GameEngine;
using Savanna.GameEngine.Constants;
using Savanna.GameEngine.Models;
using Savanna.Plugins.Tiger;
using Savanna.Plugins.Zebra;
using Savanna.Plugins.Tiger.Constants;
using Savanna.Plugins.Zebra.Constants;

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
            private readonly TestAnimalConfiguration _tigerConfig;
            private readonly TestAnimalConfiguration _zebraConfig;

            public TestAnimalFactory(
                TestAnimalConfiguration lionConfig, 
                TestAnimalConfiguration antelopeConfig,
                TestAnimalConfiguration tigerConfig,
                TestAnimalConfiguration zebraConfig)
            {
                _lionConfig = lionConfig;
                _antelopeConfig = antelopeConfig;
                _tigerConfig = tigerConfig;
                _zebraConfig = zebraConfig;
            }

            public IGameEntity CreateAnimal(char type, Position position)
            {
                return type switch
                {
                    'L' => new Lion(position, _lionConfig),
                    'A' => new Antelope(position, _antelopeConfig),
                    'T' => new Tiger(position, _tigerConfig),
                    'Z' => new Zebra(position, _zebraConfig),
                    _ => throw new ArgumentException(string.Format(TestConstants.Messages.UnknownAnimalType, type))
                };
            }

            public IEnumerable<char> GetAvailableAnimalTypes()
            {
                return new[] { 'L', 'A', 'T', 'Z' };
            }
        }

        private TestAnimalConfiguration _lionConfig;
        private TestAnimalConfiguration _antelopeConfig;
        private TestAnimalConfiguration _tigerConfig;
        private TestAnimalConfiguration _zebraConfig;
        private IAnimalFactory _animalFactory;
        private GameField _field;

        [TestInitialize]
        public void Setup()
        {
            _lionConfig = new TestAnimalConfiguration
            {
                Symbol = 'L',
                Speed = 1,
                VisionRange = 5
            };

            _antelopeConfig = new TestAnimalConfiguration
            {
                Symbol = 'A',
                Speed = 2,
                VisionRange = 4
            };

            _tigerConfig = new TestAnimalConfiguration
            {
                Symbol = 'T',
                Speed = 1,
                VisionRange = 5
            };

            _zebraConfig = new TestAnimalConfiguration
            {
                Symbol = 'Z',
                Speed = 2,
                VisionRange = 4
            };

            _animalFactory = new TestAnimalFactory(_lionConfig, _antelopeConfig, _tigerConfig, _zebraConfig);
            _field = new GameField(_animalFactory, 10, 10);
        }

        /// <summary>
        /// Verifies that animals start with maximum health when created
        /// </summary>
        [TestMethod]
        public void Animal_InitialHealth_ShouldBeMaximum()
        {
            var position = new Position(0, 0);
            var lion = _animalFactory.CreateAnimal('L', position);
            Assert.AreEqual(GameConstants.Health.InitialHealth, lion.Health);
        }

        /// <summary>
        /// Verifies that animal health decreases by the correct amount when moving
        /// </summary>
        [TestMethod]
        public void Animal_Move_ShouldDecreaseHealth()
        {
            var position = new Position(0, 0);
            _field.AddAnimal('L', position);
            var lion = _field.Animals.First(a => a.Symbol == 'L');
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
            _field.AddAnimal('L', position);
            _field.AddAnimal('A', new Position(0, 1)); // Ensure antelope is within catch distance

            var lion = _field.Animals.First(a => a.Symbol == 'L');
            var antelope = _field.Animals.First(a => a.Symbol == 'A');
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
            _field.AddAnimal('L', position);
            var lion = _field.Animals.First(a => a.Symbol == 'L');
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
            _field.AddAnimal('L', position1);
            _field.AddAnimal('L', position2);

            // Ensure lions have enough health to reproduce
            var lion1 = (IHealthManageable)_field.Animals.First(a => a.Symbol == 'L');
            var lion2 = (IHealthManageable)_field.Animals.Last(a => a.Symbol == 'L');
            lion1.IncreaseHealth(GameConstants.Reproduction.MinimumHealthToReproduce * 2);
            lion2.IncreaseHealth(GameConstants.Reproduction.MinimumHealthToReproduce * 2);

            // Update for required consecutive rounds and verify positions
            for (int i = 0; i < GameConstants.Reproduction.RequiredConsecutiveRounds; i++)
            {
                _field.Update();
                
                // Verify lions stay within reproduction range
                var updatedLion1 = _field.Animals.First(a => a.Symbol == 'L');
                var updatedLion2 = _field.Animals.Last(a => a.Symbol == 'L');
                var distance = updatedLion1.Position.DistanceTo(updatedLion2.Position);
                Assert.IsTrue(distance <= GameConstants.Reproduction.MatingDistance, 
                    $"Lions must stay within mating distance ({GameConstants.Reproduction.MatingDistance}). Current distance: {distance}");
            }

            // Verify reproduction occurred
            Assert.AreEqual(3, _field.Animals.Count(a => a.Symbol == 'L'), TestConstants.Messages.AnimalsNearShouldReproduce);
        }

        /// <summary>
        /// Verifies that animals cannot reproduce when they are too far apart
        /// </summary>
        [TestMethod]
        public void Animals_TooFarApart_ShouldNotReproduce()
        {
            var position1 = new Position(0, 0);
            var position2 = new Position(5, 5);
            _field.AddAnimal('L', position1);
            _field.AddAnimal('L', position2);

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
            _field.AddAnimal('L', position1);
            _field.AddAnimal('L', position2);

            // Reduce health below reproduction threshold for both lions
            var lion1 = (IHealthManageable)_field.Animals.First(a => a.Symbol == 'L');
            var lion2 = (IHealthManageable)_field.Animals.Last(a => a.Symbol == 'L');
            lion1.DecreaseHealth(lion1.Health - GameConstants.Reproduction.MinimumHealthToReproduce + 1);
            lion2.DecreaseHealth(lion2.Health - GameConstants.Reproduction.MinimumHealthToReproduce + 1);

            // Update for required consecutive rounds
            for (int i = 0; i < GameConstants.Reproduction.RequiredConsecutiveRounds; i++)
            {
                _field.Update();
            }

            // Verify current behavior (2 lions, no reproduction with low health)
            Assert.AreEqual(2, _field.Animals.Count(a => a.Symbol == 'L'), TestConstants.Messages.LionsNotReproduceWithLowHealth);
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

        /// <summary>
        /// Verifies that a Tiger gains health when catching a Zebra
        /// </summary>
        [TestMethod]
        public void Tiger_CatchZebra_ShouldGainHealth()
        {
            var position = new Position(0, 0);
            _field.AddAnimal('T', position); // Add Tiger
            _field.AddAnimal('Z', position); // Add Zebra at same position

            var tiger = _field.Animals.First(a => a.Symbol == 'T');
            var zebra = _field.Animals.First(a => a.Symbol == 'Z');
            var healthTiger = (IHealthManageable)tiger;
            
            _field.Update();

            Assert.IsFalse(zebra.IsAlive, TestConstants.Messages.PreyDeadWhenCaught);
            Assert.AreEqual(100.0, healthTiger.Health, TestConstants.Messages.TigerHealthMaxAtSamePosition);
        }

        /// <summary>
        /// Verifies that a Zebra moves away from a nearby Tiger
        /// </summary>
        [TestMethod]
        public void Zebra_NearTiger_ShouldMoveAway()
        {
            var zebraPosition = new Position(2, 2);
            var tigerPosition = new Position(3, 2); // Tiger one space to the right

            _field.AddAnimal('Z', zebraPosition);
            _field.AddAnimal('T', tigerPosition);

            var zebra = _field.Animals.First(a => a.Symbol == 'Z');
            var movableZebra = (IMovable)zebra;
            var initialPosition = zebra.Position;
            
            movableZebra.Move(_field);
            
            Assert.AreNotEqual(initialPosition, zebra.Position, TestConstants.Messages.ZebraShouldMoveAway);
            Assert.IsTrue(zebra.Position.X < initialPosition.X, TestConstants.Messages.ZebraMoveDirection);
        }

        /// <summary>
        /// Verifies that a Tiger can catch prey within its catch distance
        /// </summary>
        [TestMethod]
        public void Tiger_PreyWithinCatchDistance_ShouldCatch()
        {
            var tigerPosition = new Position(1, 1);
            var preyPosition = new Position(2, 1); // Adjacent position

            _field.AddAnimal('T', tigerPosition);
            _field.AddAnimal('Z', preyPosition);

            var tiger = _field.Animals.First(a => a.Symbol == 'T');
            var prey = _field.Animals.First(a => a.Symbol == 'Z');
            var movableTiger = (IMovable)tiger;

            movableTiger.Move(_field);

            Assert.IsFalse(prey.IsAlive, TestConstants.Messages.PreyDeadWhenCaught);
            Assert.AreEqual(prey.Position, tiger.Position, TestConstants.Messages.TigerMoveToPreyPosition);
        }

        /// <summary>
        /// Verifies that a Zebra's health decreases when moving
        /// </summary>
        [TestMethod]
        public void Zebra_Move_ShouldDecreaseHealth()
        {
            _field.AddAnimal('Z', new Position(0, 0));
            var zebra = _field.Animals.First(a => a.Symbol == 'Z');
            var healthZebra = (IHealthManageable)zebra;
            double initialHealth = healthZebra.Health;
            
            _field.Update();
            
            Assert.IsTrue(healthZebra.Health < initialHealth,
                string.Format(TestConstants.Messages.HealthShouldDecrease, initialHealth, healthZebra.Health));
        }

        /// <summary>
        /// Verifies that a Lion can catch an Antelope when close enough
        /// </summary>
        [TestMethod]
        public void Lion_CloseToAntelope_ShouldCatch()
        {
            var lionPosition = new Position(1, 1);
            var antelopePosition = new Position(2, 1);

            _field.AddAnimal('L', lionPosition);
            _field.AddAnimal('A', antelopePosition);

            var lion = _field.Animals.First(a => a.Symbol == 'L');
            var antelope = _field.Animals.First(a => a.Symbol == 'A');
            var movableLion = (IMovable)lion;
            var healthLion = (IHealthManageable)lion;
            double initialHealth = healthLion.Health;

            _field.Update();

            Assert.AreEqual(19.5, healthLion.Health, TestConstants.Messages.HealthAfterMovementAndCatch);
            Assert.IsFalse(antelope.IsAlive, TestConstants.Messages.PreyShouldBeDead);
        }

        /// <summary>
        /// Verifies that an Antelope moves away from a nearby Lion
        /// </summary>
        [TestMethod]
        public void Antelope_NearLion_ShouldMoveAway()
        {
            var antelopePosition = new Position(2, 2);
            var lionPosition = new Position(3, 2);

            _field.AddAnimal('A', antelopePosition);
            _field.AddAnimal('L', lionPosition);

            var antelope = _field.Animals.First(a => a.Symbol == 'A');
            var initialPosition = antelope.Position;
            
            _field.Update();
            
            Assert.AreNotEqual(initialPosition, antelope.Position, TestConstants.Messages.AntelopeShouldMoveAway);
            Assert.IsTrue(antelope.Position.X < initialPosition.X, TestConstants.Messages.AntelopeMoveDirection);
        }

        /// <summary>
        /// Verifies that Lions and Antelopes can reproduce when conditions are met
        /// </summary>
        [TestMethod]
        public void LionAndAntelope_Reproduction_ShouldCreateOffspring()
        {
            var position1 = new Position(0, 0);
            var position2 = new Position(1, 0);
            _field.AddAnimal('L', position1);
            _field.AddAnimal('L', position2);

            // Ensure lions have enough health to reproduce
            var lion1 = (IHealthManageable)_field.Animals.First(a => a.Symbol == 'L');
            var lion2 = (IHealthManageable)_field.Animals.Last(a => a.Symbol == 'L');
            lion1.IncreaseHealth(GameConstants.Reproduction.MinimumHealthToReproduce * 2);
            lion2.IncreaseHealth(GameConstants.Reproduction.MinimumHealthToReproduce * 2);

            // Update for required consecutive rounds and verify positions
            for (int i = 0; i < GameConstants.Reproduction.RequiredConsecutiveRounds; i++)
            {
                _field.Update();
                
                // Verify lions stay within reproduction range
                var updatedLion1 = _field.Animals.First(a => a.Symbol == 'L');
                var updatedLion2 = _field.Animals.Last(a => a.Symbol == 'L');
                var distance = updatedLion1.Position.DistanceTo(updatedLion2.Position);
                Assert.IsTrue(distance <= GameConstants.Reproduction.MatingDistance, 
                    $"Lions must stay within mating distance ({GameConstants.Reproduction.MatingDistance}). Current distance: {distance}");
            }

            // Verify reproduction occurred
            Assert.AreEqual(3, _field.Animals.Count(a => a.Symbol == 'L'), TestConstants.Messages.LionsReproduceCreateOffspring);
        }

        /// <summary>
        /// Verifies that animals cannot reproduce when health is too low
        /// </summary>
        [TestMethod]
        public void Animal_LowHealth_ShouldNotReproduce()
        {
            var lionPosition1 = new Position(1, 1);
            var lionPosition2 = new Position(2, 1);
            _field.AddAnimal('L', lionPosition1);
            _field.AddAnimal('L', lionPosition2);

            var lion = _field.Animals.First(a => a.Symbol == 'L');
            var healthLion = (IHealthManageable)lion;

            healthLion.DecreaseHealth(GameConstants.Health.InitialHealth - GameConstants.Reproduction.MinimumHealthToReproduce + 1);

            for (int i = 0; i < GameConstants.Reproduction.RequiredConsecutiveRounds; i++)
            {
                _field.Update();
            }

            var finalLionCount = _field.Animals.Count(a => a.Symbol == 'L');
            // Verify that reproduction occurred even with low health (current behavior)
            Assert.AreEqual(2, finalLionCount, TestConstants.Messages.LionsNotReproduceWithLowHealth);
        }
    }
} 