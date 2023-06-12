using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.WaveFunctionCollapse;
using FrigidRogue.WaveFunctionCollapse.Options;
using FrigidRogue.WaveFunctionCollapse.Renderers;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Maps;
using Microsoft.Xna.Framework.Graphics;
using NSubstitute;
using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Components.GameWorldTests;

[TestClass]
public class MapGeneratorTests : BaseGameWorldIntegrationTests
{
    private MapGenerator _mapGenerator;
    private IWaveFunctionCollapseGeneratorPasses _waveFunctionCollapseGeneratorPasses;

    [TestInitialize]
    public override void Setup()
    {
        base.Setup();

        _waveFunctionCollapseGeneratorPasses = Substitute.For<IWaveFunctionCollapseGeneratorPasses>();
        _mapGenerator = new MapGenerator(_waveFunctionCollapseGeneratorPasses, null, null);
    }

    [TestMethod]
    public void Should_CreateMiningFacilityMap()
    {
        // Arrange
        var texture2D = new Texture2D(_gameProvider.Game.GraphicsDevice, 5, 5);

        var colours = new Color[25].Initialise(() => Color.White);

        colours[6] = Color.Black;
        colours[7] = Color.Black;
        colours[8] = Color.Black;
        
        colours[11] = Color.Black;
        colours[13] = Color.Black;

        colours[16] = Color.Black;
        colours[17] = Color.Black;
        colours[18] = Color.Black;
        
        texture2D.SetData(colours);

        var mapOptions = new MapOptions(0, 0);
        _waveFunctionCollapseGeneratorPasses.MapOptions.Returns(mapOptions);

        _waveFunctionCollapseGeneratorPasses
            .RenderToTexture2D(Arg.Any<IWaveFunctionCollapseGeneratorPassesRenderer>())
            .Returns(texture2D);

        _waveFunctionCollapseGeneratorPasses.TileWidth.Returns(5);
        _waveFunctionCollapseGeneratorPasses.TileHeight.Returns(5);

        // Act
        _mapGenerator.CreateMiningFacilityMap(_gameWorld, _gameWorld.GameObjectFactory, 5, 5);

        // Assert
        var map = _mapGenerator.Map;

        var doors = map.Entities
            .Select(e => e.Item)
            .OfType<Door>()
            .ToList();
        
        Assert.AreEqual(8, map.Walls.Count + doors.Count);

        var validWallDoorPositions = new List<Point>
        {
            new (1, 1),
            new (2, 1),
            new (3, 1),
            new (1, 2),
            new (3, 2),
            new (1, 3),
            new (2, 3),
            new (3, 3)
        };
        
        foreach (var wall in map.Walls)
        {
            Assert.IsTrue(validWallDoorPositions.Contains(wall.Position));
        }

        foreach (var door in doors)
        {
            Assert.IsTrue(validWallDoorPositions.Contains(door.Position));
        }
    }
}