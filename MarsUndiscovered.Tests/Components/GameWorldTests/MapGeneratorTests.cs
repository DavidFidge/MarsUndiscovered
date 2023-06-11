using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.WaveFunctionCollapse;
using FrigidRogue.WaveFunctionCollapse.Options;
using FrigidRogue.WaveFunctionCollapse.Renderers;
using MarsUndiscovered.Game.Components.Maps;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NSubstitute;

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
        var texture2D = new Texture2D(_gameProvider.Game.GraphicsDevice, 3, 3);

        var colours = new Color[9].Initialise(() => Color.White);

        colours[3] = Color.Black;
        colours[4] = Color.Black;
        colours[5] = Color.Black;

        texture2D.SetData(colours);

        var mapOptions = new MapOptions(0, 0);
        _waveFunctionCollapseGeneratorPasses.MapOptions.Returns(mapOptions);

        _waveFunctionCollapseGeneratorPasses
            .RenderToTexture2D(Arg.Any<IWaveFunctionCollapseGeneratorPassesRenderer>())
            .Returns(texture2D);

        _waveFunctionCollapseGeneratorPasses.TileWidth.Returns(3);
        _waveFunctionCollapseGeneratorPasses.TileHeight.Returns(3);

        // Act
        _mapGenerator.CreateMiningFacilityMap(_gameWorld, _gameWorld.GameObjectFactory, 3, 3);

        // Assert
        var map = _mapGenerator.Map;

        Assert.AreEqual(3, map.Walls.Count);

        Assert.AreEqual(new SadRogue.Primitives.Point(0, 1), map.Walls[0].Position);
        Assert.AreEqual(new SadRogue.Primitives.Point(1, 1), map.Walls[1].Position);
        Assert.AreEqual(new SadRogue.Primitives.Point(2, 1), map.Walls[2].Position);
    }
}