using FrigidRogue.WaveFunctionCollapse;
using GoRogue.Random;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.GenerationSteps;
using MarsUndiscovered.Game.Components.Maps;
using NSubstitute;
using ShaiRandom.Generators;

namespace MarsUndiscovered.Tests.Components.GameWorldTests;

[TestClass]
public class PrefabMapGeneratorTests : BaseGameWorldIntegrationTests
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
    public void Should_CreatePrefabMap()
    {
        // Act
        _mapGenerator.CreatePrefabMap(_gameWorld, _gameWorld.GameObjectFactory, 50, 50, upToStep: 3);
        
        // var oldRandom = GlobalRandom.DefaultRNG;
        //GlobalRandom.DefaultRNG = new KnownSeriesRandom(new int[] { 3, 6, 2, 4 });

        // Assert
        var map = _mapGenerator.Map;
        Assert.IsNotNull(map);

        var mapText = GameObjectWriter.WriteMapAsciiArray(map);
        GameObjectWriter.WriteMapAsciiToFile(map);
    }
}