using GoRogue.MapGeneration;
using GoRogue.MapGeneration.ContextComponents;

using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.GenerationSteps;
using MarsUndiscovered.Game.Components.Maps;
using NSubstitute;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using ShaiRandom.Generators;

namespace MarsUndiscovered.Tests.Components.GameWorldTests;

[TestClass]
public class InternalWallsGenerationTests : BaseTest
{
    [TestMethod]
    public void Should_Split_Area_With_A_Vertical_Wall()
    {
        // Arrange
        var expectedResults = new[]
        {
            ".#..",
            ".D..",
            ".#..",
        };

        var mapTemplate = new MapTemplate(expectedResults);

        var internalWallsGeneration = new InternalWallsGeneration(WallType.RockWall, null, 4);
        var context = new GenerationContext(expectedResults[0].Length, expectedResults.Length);

        var random = Substitute.For<IEnhancedRandom>();
        internalWallsGeneration.RNG = random;
        
        // Point at which to split
        random
            .NextInt(Arg.Any<int>(), Arg.Any<int>())
            .Returns(1);

        // Index for door
        random.NextInt(Arg.Any<int>()).Returns(1);
        
        var wallsFloorTypes = context.GetFirstOrNew<ArrayView<GameObjectType>>(
            () => new ArrayView<GameObjectType>(context.Width, context.Height),
            MapGenerator.WallFloorTypeTag);

        for (var index = 0; index < wallsFloorTypes.Count; index++)
            wallsFloorTypes[index] = FloorType.BlankFloor;

        var areaItemList = new ItemList<Area>();
        
        var area = new Area(mapTemplate.Select(m => m.Point).ToList());
        areaItemList.Add(area, null);
        
        context
            .GetFirstOrNew(() => areaItemList, MapGenerator.AreasTag)
            .ToList();
        
        // Act
        internalWallsGeneration.PerformStep(context);

        // Assert
        var results = context.GetFirst<ItemList<AreaWallsDoors>>().ToList();

        Assert.AreEqual(1, results.Count);
        Assert.IsTrue(results.All(s => s.Step == "InternalWallsGeneration"));

        var areaWallsDoors = results.First().Item;

        foreach (var wall in areaWallsDoors.Walls)
        {
            Assert.AreEqual('#', mapTemplate.GetCharAt(wall));
        }
        
        foreach (var door in areaWallsDoors.Doors)
        {
            Assert.AreEqual('D', mapTemplate.GetCharAt(door));
        }
    }
    
    [TestMethod]
    public void Should_Split_Area_With_A_Horizontal_Wall()
    {
        // Arrange
        var expectedResults = new[]
        {
            "...",
            "#D#",
            "...",
            "..."
        };

        var mapTemplate = new MapTemplate(expectedResults);

        var internalWallsGeneration = new InternalWallsGeneration(WallType.RockWall, null, 4);
        var context = new GenerationContext(expectedResults[0].Length, expectedResults.Length);

        var random = Substitute.For<IEnhancedRandom>();
        internalWallsGeneration.RNG = random;
        
        // Point at which to split
        random
            .NextInt(Arg.Any<int>(), Arg.Any<int>())
            .Returns(1);

        // Index for door
        random.NextInt(Arg.Any<int>()).Returns(1);
        
        var wallsFloorTypes = context.GetFirstOrNew<ArrayView<GameObjectType>>(
            () => new ArrayView<GameObjectType>(context.Width, context.Height),
            MapGenerator.WallFloorTypeTag);

        for (var index = 0; index < wallsFloorTypes.Count; index++)
            wallsFloorTypes[index] = FloorType.BlankFloor;

        var areaItemList = new ItemList<Area>();
        
        var area = new Area(mapTemplate.Select(m => m.Point).ToList());
        areaItemList.Add(area, null);
        
        context
            .GetFirstOrNew(() => areaItemList, MapGenerator.AreasTag)
            .ToList();
        
        // Act
        internalWallsGeneration.PerformStep(context);

        // Assert
        var results = context.GetFirst<ItemList<AreaWallsDoors>>().ToList();

        Assert.AreEqual(1, results.Count);
        Assert.IsTrue(results.All(s => s.Step == "InternalWallsGeneration"));

        var areaWallsDoors = results.First().Item;

        foreach (var wall in areaWallsDoors.Walls)
        {
            Assert.AreEqual('#', mapTemplate.GetCharAt(wall));
        }
        
        foreach (var door in areaWallsDoors.Doors)
        {
            Assert.AreEqual('D', mapTemplate.GetCharAt(door));
        }
    }
    
    // TODO - need a test to ensure that a wall cannot be built on top of a door
}