using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.WaveFunctionCollapse;
using FrigidRogue.WaveFunctionCollapse.Options;
using FrigidRogue.WaveFunctionCollapse.Renderers;
using MarsUndiscovered.Game.Components.GenerationSteps;
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
        _mapGenerator = new MapGenerator(_waveFunctionCollapseGeneratorPasses, null, null,
            new PrefabProvider());
    }

    [TestMethod]
    public void Should_CreateMiningFacilityMap()
    {
        // Arrange
        var texture2D = new Texture2D(_gameProvider.Game.GraphicsDevice, 5, 5);

        var colours = new Color[25].Initialise(() => Color.White);

        for (var i = 0; i < 15; i++)
        {
            colours[i] = Color.Blue;
        }
        
        for (var i = 15; i < 25; i++)
        {
            colours[i] = Color.Red;
        }

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
        Assert.IsNotNull(map);
    }
}