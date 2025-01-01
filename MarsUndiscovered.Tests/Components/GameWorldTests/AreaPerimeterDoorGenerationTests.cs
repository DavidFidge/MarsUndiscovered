using GoRogue.MapGeneration;
using GoRogue.MapGeneration.ContextComponents;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.GenerationSteps;
using MarsUndiscovered.Game.Components.Maps;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using ShaiRandom.Generators;

namespace MarsUndiscovered.Tests.Components.GameWorldTests;

[TestClass]
public class AreaPerimeterDoorGenerationTests : BaseTest
{
    [TestMethod]
    public void Should_Not_Create_Doors_On_Edge_Of_Map()
    {
        // Arrange
        var expectedResults = new[]
        {
            "###",
            "#.#",
            "###",
        };

        // Only used for generating the area
        var areaMap = new[]
        {
            "###",
            "###",
            "###",
        };
        
        var mapTemplate = new MapTemplate(expectedResults);
        var areaMapTemplate = new MapTemplate(areaMap);

        var areaPerimeterDoorGeneration = new AreaPerimeterDoorGeneration(FloorType.BlankFloor, DoorType.DefaultDoor, null, 1, 5);
        
        var context = new GenerationContext(expectedResults[0].Length, expectedResults.Length);

        var random = new KnownSeriesRandom(new[]
        {
            1,// create one door
            0 // choose first valid position 
        });
        
        areaPerimeterDoorGeneration.RNG = random;
        
        var wallsFloorTypes = context.GetFirstOrNew(
            () => new ArrayView<GameObjectType>(context.Width, context.Height),
            MapGenerator.WallFloorTypeTag);

        for (var index = 0; index < wallsFloorTypes.Count; index++)
            wallsFloorTypes[index] = FloorType.BlankFloor;
        
        foreach (var item in mapTemplate.Where(i => i.Char == '#'))
            wallsFloorTypes[item.Point] = WallType.RockWall;

        var areaItemList = new ItemList<Area>();
        
        var area = new Area(areaMapTemplate
            .Where(m => m.Char == '#')
            .Select(m => m.Point)
            .ToList());
        
        areaItemList.Add(area, null);
        
        context
            .GetFirstOrNew(() => areaItemList, MapGenerator.AreasTag)
            .ToList();
        
        // Act
        areaPerimeterDoorGeneration.PerformStep(context);

        // Assert
        var results = context.GetFirst<ItemList<GameObjectTypePosition<DoorType>>>().ToList();

        Assert.AreEqual(0, results.Count);
    }
    
    [TestMethod]
    public void Should_Create_Doors_on_Area_Perimeter()
    {
        // Arrange
        var expectedResults = new[]
        {
            ".....",
            ".###.",
            ".#.#.",
            ".###.",
            "....."
        };

        // Only used for generating the area
        var areaMap = new[]
        {
            ".....",
            ".###.",
            ".###.",
            ".###.",
            "....."
        };
        
        var mapTemplate = new MapTemplate(expectedResults);
        var areaMapTemplate = new MapTemplate(areaMap);

        var areaPerimeterDoorGeneration = new AreaPerimeterDoorGeneration(FloorType.BlankFloor, DoorType.DefaultDoor, null, 1, 5);
        
        var context = new GenerationContext(expectedResults[0].Length, expectedResults.Length);

        var random = new KnownSeriesRandom(new[]
        {
            1,// create one door
            0 // choose first valid position 
        });
        
        areaPerimeterDoorGeneration.RNG = random;
        
        var wallsFloorTypes = context.GetFirstOrNew(
            () => new ArrayView<GameObjectType>(context.Width, context.Height),
            MapGenerator.WallFloorTypeTag);

        for (var index = 0; index < wallsFloorTypes.Count; index++)
            wallsFloorTypes[index] = FloorType.BlankFloor;
        
        foreach (var item in mapTemplate.Where(i => i.Char == '#'))
            wallsFloorTypes[item.Point] = WallType.RockWall;

        var areaItemList = new ItemList<Area>();
        
        var area = new Area(areaMapTemplate
            .Where(m => m.Char == '#')
            .Select(m => m.Point)
            .ToList());
        
        areaItemList.Add(area, null);
        
        context
            .GetFirstOrNew(() => areaItemList, MapGenerator.AreasTag)
            .ToList();
        
        // Act
        areaPerimeterDoorGeneration.PerformStep(context);

        // Assert
        var results = context.GetFirst<ItemList<GameObjectTypePosition<DoorType>>>().ToList();

        Assert.AreEqual(1, results.Count);
        Assert.IsTrue(results.All(s => s.Step == "AreaPerimeterDoorGeneration"));

        var door = results.First().Item;
        
        Assert.AreEqual(DoorType.DefaultDoor, door.GameObjectType);
        Assert.AreEqual(FloorType.BlankFloor, wallsFloorTypes[door.Position]);
        Assert.AreEqual(new Point(2, 1), door.Position);
    }
    
    [TestMethod]
    public void Should_Not_Create_Doors_Leading_Into_Walls()
    {
        // Arrange
        var expectedResults = new[]
        {
            ".....",
            ".###.",
            ".###.",
            ".###.",
            "....."
        };

        // Only used for generating the area
        var areaMap = new[]
        {
            ".....",
            ".###.",
            ".###.",
            ".###.",
            "....."
        };
        
        var mapTemplate = new MapTemplate(expectedResults);
        var areaMapTemplate = new MapTemplate(areaMap);

        var areaPerimeterDoorGeneration = new AreaPerimeterDoorGeneration(FloorType.BlankFloor, DoorType.DefaultDoor, null, 1, 5);
        
        var context = new GenerationContext(expectedResults[0].Length, expectedResults.Length);

        var random = new KnownSeriesRandom(new[]
        {
            1,// create one door
            0 // choose first valid position 
        });
        
        areaPerimeterDoorGeneration.RNG = random;
        
        var wallsFloorTypes = context.GetFirstOrNew(
            () => new ArrayView<GameObjectType>(context.Width, context.Height),
            MapGenerator.WallFloorTypeTag);

        for (var index = 0; index < wallsFloorTypes.Count; index++)
            wallsFloorTypes[index] = FloorType.BlankFloor;
        
        foreach (var item in mapTemplate.Where(i => i.Char == '#'))
            wallsFloorTypes[item.Point] = WallType.RockWall;

        var areaItemList = new ItemList<Area>();
        
        var area = new Area(areaMapTemplate
            .Where(m => m.Char == '#')
            .Select(m => m.Point)
            .ToList());
        
        areaItemList.Add(area, null);
        
        context
            .GetFirstOrNew(() => areaItemList, MapGenerator.AreasTag)
            .ToList();
        
        // Act
        areaPerimeterDoorGeneration.PerformStep(context);

        // Assert
        var results = context.GetFirst<ItemList<GameObjectTypePosition<DoorType>>>().ToList();

        Assert.AreEqual(0, results.Count);
    }

    [TestMethod]
    public void Should_Only_Create_Doors_On_Walls_With_Free_Squares_On_The_Vertical_Or_Horizontal()
    {
        // Arrange
        var expectedResults = new[]
        {
            ".....",
            ".###.",
            ".#.#.",
            ".###.",
            "....."
        };

        // Only used for generating the area
        var areaMap = new[]
        {
            ".....",
            ".###.",
            ".###.",
            ".###.",
            "....."
        };
        
        var mapTemplate = new MapTemplate(expectedResults);
        var areaMapTemplate = new MapTemplate(areaMap);

        var areaPerimeterDoorGeneration = new AreaPerimeterDoorGeneration(FloorType.BlankFloor, DoorType.DefaultDoor, null, 1, 10);
        
        var context = new GenerationContext(expectedResults[0].Length, expectedResults.Length);

        var random = new KnownSeriesRandom(new[]
        {
            5, // Return 5 doors.  There will only be room for 4
            0, // Choose first index for door in list 
            0, // Choose first index for door in list 
            0, // Choose first index for door in list 
            0, // Choose first index for door in list 
            0, // Choose first index for door in list - this one shouldn't be run
        });
        
        areaPerimeterDoorGeneration.RNG = random;
        
        var wallsFloorTypes = context.GetFirstOrNew(
            () => new ArrayView<GameObjectType>(context.Width, context.Height),
            MapGenerator.WallFloorTypeTag);

        for (var index = 0; index < wallsFloorTypes.Count; index++)
            wallsFloorTypes[index] = FloorType.BlankFloor;
        
        foreach (var item in mapTemplate.Where(i => i.Char == '#'))
            wallsFloorTypes[item.Point] = WallType.RockWall;

        var areaItemList = new ItemList<Area>();
        
        var area = new Area(areaMapTemplate
            .Where(m => m.Char == '#')
            .Select(m => m.Point)
            .ToList());
        
        areaItemList.Add(area, null);
        
        context
            .GetFirstOrNew(() => areaItemList, MapGenerator.AreasTag)
            .ToList();
        
        // Act
        areaPerimeterDoorGeneration.PerformStep(context);

        // Assert
        var results = context.GetFirst<ItemList<GameObjectTypePosition<DoorType>>>().ToList();

        Assert.AreEqual(4, results.Count);
        Assert.IsTrue(results.All(s => s.Step == "AreaPerimeterDoorGeneration"));

        var door1 = results[0].Item;
        var door2 = results[1].Item;
        var door3 = results[2].Item;
        var door4 = results[3].Item;
        
        Assert.AreEqual(DoorType.DefaultDoor, door1.GameObjectType);
        Assert.AreEqual(FloorType.BlankFloor, wallsFloorTypes[door1.Position]);
        Assert.AreEqual(new Point(2, 1), door1.Position);
        
        Assert.AreEqual(DoorType.DefaultDoor, door2.GameObjectType);
        Assert.AreEqual(FloorType.BlankFloor, wallsFloorTypes[door2.Position]);
        Assert.AreEqual(new Point(1, 2), door2.Position);
        
        Assert.AreEqual(DoorType.DefaultDoor, door3.GameObjectType);
        Assert.AreEqual(FloorType.BlankFloor, wallsFloorTypes[door3.Position]);
        Assert.AreEqual(new Point(3, 2), door3.Position);
        
        Assert.AreEqual(DoorType.DefaultDoor, door4.GameObjectType);
        Assert.AreEqual(FloorType.BlankFloor, wallsFloorTypes[door4.Position]);
        Assert.AreEqual(new Point(2, 3), door4.Position);
    }
    
    [TestMethod]
    public void Should_Create_Doors_On_Multiple_Areas()
    {
        // Arrange
        var expectedResults = new[]
        {
            "..........",
            ".###..###.",
            ".#.#..#.#.",
            ".###..###.",
            ".........."
        };

        // Only used for generating the area

        var areaMap1 = new[]
        {
            "..........",
            ".###......",
            ".###......",
            ".###......",
            ".........."
        };
        var areaMap2 = new[]
        {
            "..........",
            "......###.",
            "......###.",
            "......###.",
            ".........."
        };
        
        var mapTemplate = new MapTemplate(expectedResults);
        var areaMapTemplate1 = new MapTemplate(areaMap1);
        var areaMapTemplate2 = new MapTemplate(areaMap2);

        var areaPerimeterDoorGeneration = new AreaPerimeterDoorGeneration(FloorType.BlankFloor, DoorType.DefaultDoor, null, 1, 5);
        
        var context = new GenerationContext(expectedResults[0].Length, expectedResults.Length);

        var random = new KnownSeriesRandom(new[]
        {
            1, // create one door
            0, // choose first valid position
            2, // create 2 doors
            0, // choose first valid position
            0 // choose first valid position
        });
        
        areaPerimeterDoorGeneration.RNG = random;
        
        var wallsFloorTypes = context.GetFirstOrNew(
            () => new ArrayView<GameObjectType>(context.Width, context.Height),
            MapGenerator.WallFloorTypeTag);

        for (var index = 0; index < wallsFloorTypes.Count; index++)
            wallsFloorTypes[index] = FloorType.BlankFloor;
        
        foreach (var item in mapTemplate.Where(i => i.Char == '#'))
            wallsFloorTypes[item.Point] = WallType.RockWall;

        var areaItemList = new ItemList<Area>();
        
        var area1 = new Area(areaMapTemplate1
            .Where(m => m.Char == '#')
            .Select(m => m.Point)
            .ToList());
        
        areaItemList.Add(area1, null);
 
        var area2 = new Area(areaMapTemplate2
            .Where(m => m.Char == '#')
            .Select(m => m.Point)
            .ToList());
        
        areaItemList.Add(area2, null);
        
        context
            .GetFirstOrNew(() => areaItemList, MapGenerator.AreasTag)
            .ToList();
        
        // Act
        areaPerimeterDoorGeneration.PerformStep(context);

        // Assert
        var results = context.GetFirst<ItemList<GameObjectTypePosition<DoorType>>>().ToList();

        Assert.AreEqual(3, results.Count);
        Assert.IsTrue(results.All(s => s.Step == "AreaPerimeterDoorGeneration"));
        
        var door1 = results[0].Item;
        var door2 = results[1].Item;
        var door3 = results[2].Item;
        
        Assert.AreEqual(DoorType.DefaultDoor, door1.GameObjectType);
        Assert.AreEqual(FloorType.BlankFloor, wallsFloorTypes[door1.Position]);
        Assert.AreEqual(new Point(2, 1), door1.Position);
        
        Assert.AreEqual(DoorType.DefaultDoor, door2.GameObjectType);
        Assert.AreEqual(FloorType.BlankFloor, wallsFloorTypes[door2.Position]);
        Assert.AreEqual(new Point(7, 1), door2.Position);
        
        Assert.AreEqual(DoorType.DefaultDoor, door3.GameObjectType);
        Assert.AreEqual(FloorType.BlankFloor, wallsFloorTypes[door3.Position]);
        Assert.AreEqual(new Point(6, 2), door3.Position);
    }
}