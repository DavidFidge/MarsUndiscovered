using FrigidRogue.WaveFunctionCollapse;
using MarsUndiscovered.Game.Components.Maps;
using NSubstitute;

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
        _mapGenerator = new MapGenerator(_waveFunctionCollapseGeneratorPasses, null, null);
    }

    [TestMethod]
    public void Should_CreateMiningFacilityMap()
    {
        // Act
        _mapGenerator.CreatePrefabMap(_gameWorld, _gameWorld.GameObjectFactory, 50, 50);

        // Assert
        var map = _mapGenerator.Map;
        Assert.IsNotNull(map);
    }
}