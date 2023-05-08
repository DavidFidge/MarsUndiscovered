using FrigidRogue.WaveFunctionCollapse;
using FrigidRogue.WaveFunctionCollapse.Renderers;
using MarsUndiscovered.Game.Components.Maps;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NSubstitute;

namespace MarsUndiscovered.Tests.Components.GameWorldTests;

[TestClass]
public class MapGeneratorTests : BaseGraphicsGameWorldIntegrationTests
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
    public void Should_C()
    {
        // Arrange
        var texture2D = new Texture2D(GraphicsDevice, 3, 3);

        var colours = new Color[9];

        for (var i = 0; i < colours.Length; i++)
            colours[i] = Color.White;

        colours[3] = Color.Black;
        colours[4] = Color.Black;
        colours[5] = Color.Black;

        texture2D.SetData(colours);

        _waveFunctionCollapseGeneratorPasses
            .RenderToTexture2D(Arg.Any<IWaveFunctionCollapseGeneratorPassesRenderer>())
            .Returns(texture2D);

        // Act
        _mapGenerator.CreateMiningFacilityMap(_gameWorld, _gameWorld.GameObjectFactory, 3, 3);

        // Assert
        var map = _mapGenerator.Map;

        Assert.AreEqual(3, map.Walls.Count);

        Assert.AreEqual(new SadRogue.Primitives.Point(1, 0), map.Walls[3].Position);
        Assert.AreEqual(new SadRogue.Primitives.Point(1, 1), map.Walls[4].Position);
        Assert.AreEqual(new SadRogue.Primitives.Point(1, 2), map.Walls[5].Position);
    }
}