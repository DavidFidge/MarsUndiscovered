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
public class AreaPerimeterDoorGenerationTests : BaseTest
{
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
            ".....",
        };

        var mapTemplate = new MapTemplate(expectedResults);

        var areaPerimeterDoorGeneration = new AreaPerimeterDoorGeneration(FloorType.BlankFloor, DoorType.DefaultDoor, null, 1, 5);
        var context = new GenerationContext(expectedResults[0].Length, expectedResults.Length);

        var random = new KnownSeriesRandom(new[]
        {
            1,// create one door
            1 // choose first valid position 
        });
        
        areaPerimeterDoorGeneration.RNG = random;
        
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
        areaPerimeterDoorGeneration.PerformStep(context);

        // Assert
        var results = context.GetFirst<ItemList<GameObjectTypePosition<DoorType>>>().ToList();

        Assert.AreEqual(1, results.Count);
        Assert.IsTrue(results.All(s => s.Step == "InternalWallsGeneration"));

        var door = results.First().Item;
        
        Assert.AreEqual(DoorType.DefaultDoor, door.GameObjectType);
        Assert.AreEqual(FloorType.BlankFloor, wallsFloorTypes[door.Position]);
    }
}