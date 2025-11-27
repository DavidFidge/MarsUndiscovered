using FrigidRogue.MonoGame.Core.Components;
using GoRogue.MapGeneration;
using GoRogue.MapGeneration.ContextComponents;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Interfaces;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Game.Components.Maps
{
    public abstract class BaseMapGenerator : BaseComponent, IMapGenerator
    {
        public static string WallFloorTag = "WallFloor"; // ArrayView of bool where true is floor and false is wall. You should keep this one maintained if changing walls and floors.
        public static string WallFloorInvertedTag = "WallFloorInverted"; // ArrayView of bool where true is wall and false is floor. You do not have to keep it maintained but if you want to use it you have to put in a WallFloorInverterGenerator call in before the generator that wants to use it.
        public static string TunnelsTag = "Tunnels";
        public static string WallFloorTypeTag = "WallFloorType"; // Similar to WallFloor but gives the actual type of wall or floor. It is an ArrayView of GameObjectType where the type is WallType or FloorType.
        public static string MiningFacilityAreaTag = "MiningFacilityArea";
        public static string MiningFacilityAreaWithPerimeterTag = "MiningFacilityAreaWithPerimeterTag";
        public static string HoleInTheWallAreaTag = "HoleInTheWallRectangle";
        public static string DoorsTag = "Doors";
        public static string AreasTag = "Areas";
        public static string AreasWallsDoorsTag = "AreasWallsDoors";
        public static string PrefabTag = "Prefabs"; // ItemList<Prefab>
        public static string RubbleTag = "RubbleTag"; // ItemList<Prefab>

        public MarsMap Map { get; set; }
        public int Steps { get; set; }
        public bool IsComplete { get; set; }

        public abstract void CreateOutdoorMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int width, int height, int? upToStep = null);

        public abstract void CreateMineMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int width, int height, int? upToStep = null);

        public abstract void CreateMiningFacilityMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int width, int height, int? upToStep = null);

        public abstract void CreatePrefabMap(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int width, int height, int? upToStep = null);
        
        // The generators create structures with tags. This method then extracts those
        // structures and creates a map, converting the structures to walls, floors and doors.
        protected void ExecuteMapSteps(IGameWorld gameWorld, IGameObjectFactory gameObjectFactory, int? upToStep,
            Generator generator, IEnumerable<GenerationStep> generationSteps)
        {
            if (upToStep == null)
            {
                generator.ConfigAndGenerateSafe(g => g.AddSteps(generationSteps));
                IsComplete = true;
            }
            else
            {
                var mapStepEnumerator = generator.ConfigAndGetStageEnumeratorSafe(g => g.AddSteps(generationSteps), 1);

                for (var i = 0; i < upToStep.Value; i++)
                {
                    Steps++;
                    var isMoreSteps = mapStepEnumerator.MoveNext();
                    if (!isMoreSteps)
                    {
                        IsComplete = true;
                        break;
                    }
                }
            }

            var wallsFloors = generator.Context
                .GetFirstOrDefault<ArrayView<GameObjectType>>(WallFloorTypeTag);

            if (wallsFloors == null)
            {
                if (upToStep == null)
                    throw new Exception("Generators did not produce an ArrayView of GameObjectType which is not allowed when not in Progressive Mode (upToStep == null)");

                var wallsFloorsBool = generator.Context
                    .GetFirst<ArrayView<bool>>(WallFloorTag)
                    .ToArray()
                    .Select(b => b ? FloorType.RockFloor : (GameObjectType)WallType.RockWall)
                    .ToArray();

                wallsFloors = new ArrayView<GameObjectType>(wallsFloorsBool, generator.Context.Width);
            }

             var walls = new List<Wall>(wallsFloors.Count);
             var floors = new List<Floor>(wallsFloors.Count);

             for (var index = 0; index < wallsFloors.Count; index++)
             {
                 var wallFloor = wallsFloors[index];

                 Terrain terrain;

                 switch (wallFloor)
                 {
                     case WallType wallType:
                     {
                         var wall = gameObjectFactory.CreateGameObject<Wall>();
                         terrain = wall;
                         wall.WallType = wallType;
                         walls.Add(wall);
                         break;
                     }
                     case FloorType floorType:
                     {
                         var floor = gameObjectFactory.CreateGameObject<Floor>();
                         terrain = floor;
                         floor.FloorType = floorType;
                         floors.Add(floor);
                         break;
                     }
                     default:
                         throw new Exception("Unknown wall/floor type");
                 }

                 terrain.Index = index;
             }
             
             var doorTypes = generator.Context
                 .GetFirstOrNew(() => new ItemList<GameObjectTypePosition<DoorType>>(), DoorsTag);

             var doors = new List<Door>(doorTypes.Count());
             
             foreach (var doorType in doorTypes.Items)
             {
                 var door = gameObjectFactory.CreateGameObject<Door>();
                 door.DoorType = doorType.GameObjectType;
                 door.Position = doorType.Position;
                 
                 doors.Add(door);
             }

             Map = CreateMap(gameWorld, generator.Context.Width, generator.Context.Height)
                .WithTerrain(walls, floors)
                .WithDoors(doors);
        }

        protected void Clear()
        {
            Map = null;
            IsComplete = false;
            Steps = 0;
        }

        public static MarsMap CreateMap(IGameWorld gameWorld, int mapWidth, int mapHeight)
        {
            var map = new MarsMap(gameWorld, mapWidth, mapHeight);
            
            return map;
        }        
    }
}
