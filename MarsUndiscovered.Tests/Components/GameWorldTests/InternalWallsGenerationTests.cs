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
            "..#.",
            "..#.",
            "..D."
        };

        var mapTemplate = new MapTemplate(expectedResults);

        var internalWallsGeneration = new InternalWallsGeneration(WallType.RockWall, DoorType.DefaultDoor, null, 4);
        var context = new GenerationContext(expectedResults[0].Length, expectedResults.Length);

        var random = new KnownSeriesRandom(new[]
        {
            1, // vertical split 
            2 // Door
        });
        
        internalWallsGeneration.RNG = random;
        
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

        var internalWallsGeneration = new InternalWallsGeneration(WallType.RockWall, DoorType.DefaultDoor, null, 4);
        var context = new GenerationContext(expectedResults[0].Length, expectedResults.Length);

        var random = new KnownSeriesRandom(new[]
        {
            0, // Horizontal split 
            1 // Door on horizontal split
        });
        
        internalWallsGeneration.RNG = random;
        
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
    public void Should_Split_Sub_Areas()
    {
        // Arrange
        var expectedResults = new[]
        {
            "..D.",
            "#D##",
            "..#.",
            "..D.",
        };

        var mapTemplate = new MapTemplate(expectedResults);

        var internalWallsGeneration = new InternalWallsGeneration(WallType.RockWall, DoorType.DefaultDoor, null, 4);
        var context = new GenerationContext(expectedResults[0].Length, expectedResults.Length);

        var random = new KnownSeriesRandom(new[]
        {
            0, // Horizontal split 
            1, // Door on horizontal split
            0, // y = 0 split
            0, // Door on y = 0 split,
            0, // y > 1 split
            1 // Door on y > 1 split
        });
        
        internalWallsGeneration.RNG = random;
        
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
    public void Should_Not_Split_If_Split_Would_Go_Over_Door()
    {
        // Arrange
        var expectedResults = new[]
        {
            "...",
            "#D#",
            "...",
            "...",
        };

        var mapTemplate = new MapTemplate(expectedResults);

        // Set split factor to 1 so that it will try to split vertically after doing horizontal split
        var internalWallsGeneration = new InternalWallsGeneration(WallType.RockWall, DoorType.DefaultDoor, null, 1);
        var context = new GenerationContext(expectedResults[0].Length, expectedResults.Length);

        var random = new KnownSeriesRandom(new[]
        {
            0, // Horizontal split 
            1 // Door on horizontal split
        });
            
        internalWallsGeneration.RNG = random;
        
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
}