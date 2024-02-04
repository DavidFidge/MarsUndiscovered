using FrigidRogue.WaveFunctionCollapse;
using MarsUndiscovered.Game.Components;
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
    public void Should_CreatePrefabMap()
    {
        // Act
        _mapGenerator.CreatePrefabMap(_gameWorld, _gameWorld.GameObjectFactory, 50, 11);

        // Assert
        var map = _mapGenerator.Map;
        Assert.IsNotNull(map);

        var mapText = GameObjectWriter.WriteMapAscii(map);

        var expectedResult = new[]
        {
            "##################################################",
            "#.......##########################################",
            "#.......###.......################################",
            "#.......###.......################################",
            "#.......###.......################################",
            "#.......###.......################################",
            "#.......###.......################################",
            "#.......###.......################################",
            "###########.......################################",
            "##################################################",
            "##################################################"
        };

        var actualResult = mapText.ToString().Split(Environment.NewLine).Reverse().Skip(1).Reverse().ToList();
        
        for (var i = 0; i < expectedResult.Length; i++)
        {
            Assert.AreEqual(expectedResult[i], actualResult[i]);
        }
    }
}