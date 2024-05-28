using GoRogue.SenseMapping;
using GoRogue.SenseMapping.Sources;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Maps;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Tests.Components.GameWorldTests
{
    [TestClass]
    public class SenseMapTests : BaseGameWorldIntegrationTests
    {
        [TestMethod]
        public void Should_Increase_Residual_Regen()
        {
            // Arrange
            // Arrange
            var lines = new[]
            {
                "..........",
                "..........",
                "..........",
                "..........",
                "..........",
                "..........",
                "..........",
                "..........",
                "..........",
                ".........."
            };

            var mapTemplate = new MapTemplate(lines);
            
            var mapGenerator = new SpecificMapGenerator(_gameWorld.GameObjectFactory, mapTemplate.Where(m => m.Char == '#').Select(m => m.Point).ToList());

            NewGameWithTestLevelGenerator(
                _gameWorld,
                mapGenerator,
                playerPosition: new Point(5, 5),
                mapWidth: mapTemplate.Bounds.Width,
                mapHeight: mapTemplate.Bounds.Height);     
            
            var arrayView = new ArrayView<double>(mapGenerator.Map.Width, mapGenerator.Map.Height);
            
            var senseSource = new RippleSenseSource(new Point(5, 5), 2, Distance.Chebyshev);
            var senseMap = new SenseMap(arrayView);
            senseMap.AddSenseSource(senseSource);
            senseMap.Calculate();

            var resultView = senseMap.ResultView;
        }
    }
}