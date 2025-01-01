using FrigidRogue.MonoGame.Core.Extensions;
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
    private IWaveFunctionCollapseGeneratorPasses _waveFunctionCollapseGeneratorPasses;

    [TestInitialize]
    public override void Setup()
    {
        base.Setup();

        _waveFunctionCollapseGeneratorPasses = Substitute.For<IWaveFunctionCollapseGeneratorPasses>();
    }

    [TestMethod]
    public void Should_CreatePrefabMap()
    {
        // Arrange
        var mapGenerator = new MapGenerator(_waveFunctionCollapseGeneratorPasses, null, null,
            new PrefabProvider());
        
        // Act
        mapGenerator.CreatePrefabMap(_gameWorld, _gameWorld.GameObjectFactory, 50, 50, upToStep: 3);

        // Assert
        var map = mapGenerator.Map;
        Assert.IsNotNull(map);

        var mapText = GameObjectWriter.WriteMapAsciiArray(map);
        GameObjectWriter.WriteMapAsciiToFile(map);
    }
    
    [TestMethod]
    public void Prefab_Maps_Should_Be_Able_To_Overlap()
    {
        var mapGenerator = new TestPrefabMapGenerator(_gameWorld.GameObjectFactory);
        
        // Act
        var oldRandom = GlobalRandom.DefaultRNG;
        
        // map:
        // #####
        // #.#.#
        // #####
        var intSeries = new[] { 0, 0, 0, 0, 0, 2, 0 };
        var rngIntSeries = new int[1000];
        intSeries.CopyTo(rngIntSeries, 0);
        
        var knownSeriesRandom = new KnownSeriesRandom(rngIntSeries,
            boolSeries: new[] { false },
            ulongSeries: new ulong[] { 0 });
        
        GlobalRandom.DefaultRNG = knownSeriesRandom;
        
        mapGenerator.CreatePrefabMap(_gameWorld, _gameWorld.GameObjectFactory, 5, 3);

        // Assert
        var map = mapGenerator.Map;
        Assert.IsNotNull(map);
        
        var mapText = GameObjectWriter.WriteMapAsciiArray(map);

        // prefabs get joined at 2, 1
        var expectedMapText =
            "#####" +
            "#...#" +
            "#####";
            
        Assert.AreEqual(expectedMapText, mapText.Join(""));
    }
}