using System.Text;

using FrigidRogue.MonoGame.Core.Extensions;

using GoRogue.Random;

using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Maps;
using MarsUndiscovered.Game.Components.Maps.MapPointChoiceRules;

using NGenerics.Extensions;

using SadRogue.Primitives;

using ShaiRandom.Generators;

namespace MarsUndiscovered.Tests.Components.GameWorldTests
{
    [TestClass]
    public class LevelGeneratorTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void Should_Not_Block_Dungeon_With_Obstacles_Walls()
        {
            // Arrange
            // Machine should only be placed at either end of the tunnel
            var lines = new[]
            {
                "##################",
                ".................#",
                "##################",
                ".................." // This line is needed to put the player on otherwise they get treated as walls and so point 1, 1 is treated as a tunnel end
            };

            var mapTemplate = new MapTemplate(lines, 0, 0);
            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, mapTemplate.Where(m => m.Char == '#').Select(m => m.Point).ToList());

            NewGameWithTestLevelGenerator(_gameWorld, mapGenerator, mapWidth: mapTemplate.Bounds.Width, mapHeight: mapTemplate.Bounds.Height, machineCount: 0, playerPosition: new Point(0, 1));

            var map = mapGenerator.Map;
            var nonBlockingRule = new NonBlockingRule();
            nonBlockingRule.AssignMap(map);
            _gameWorld.Player.Position = new Point(0, 3);

            // Act and Assert
            for (var i = 0; i < map.Width - 1; i++)
            {
                var point = new Point(i, 1);

                var result = nonBlockingRule.IsValid(point);

                if (i == map.Width - 2 || i == 0)
                    Assert.IsTrue(result);
                else
                    Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public void Should_Not_Block_Dungeon_With_Obstacles_Walls_And_NonWalkable_Objects()
        {
            // Arrange
            // Machine should only be placed at either end of the tunnel
            // M will be floors but will have machines placed on them later in the test
            // machines cannot be moved past and should be treated as blocking
            var lines = new[]
            {
                "#MMM#",
                "....#",
                "#####",
                "....." // This line is needed to put the player on otherwise they get treated as walls and so point 1, 1 is treated as a tunnel end
            };

            var mapTemplate = new MapTemplate(lines, 0, 0);
            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, mapTemplate.Where(m => m.Char == '#').Select(m => m.Point).ToList());

            NewGameWithTestLevelGenerator(_gameWorld, mapGenerator, mapWidth: mapTemplate.Bounds.Width, mapHeight: mapTemplate.Bounds.Height, machineCount: 0, playerPosition: new Point(0, 1));

            for (var x = 1; x <= 3; x++)
            {
                var machineParams = new SpawnMachineParams()
                .WithMachineType(MachineType.Analyzer)
                .AtPosition(new Point(x, 0))
                .OnMap(_gameWorld.CurrentMap.Id);

                _gameWorld.SpawnMachine(machineParams);
            }

            var map = _gameWorld.CurrentMap;
            var nonBlockingRule = new NonBlockingRule();
            nonBlockingRule.AssignMap(map);
            _gameWorld.Player.Position = new Point(0, 3);
            // Act and Assert
            for (var i = 0; i < map.Width - 1; i++)
            {
                var point = new Point(i, 1);

                var result = nonBlockingRule.IsValid(point);

                if (i == map.Width - 2 || i == 0)
                    Assert.IsTrue(result);
                else
                    Assert.IsFalse(result);
            }
        }
    }
}