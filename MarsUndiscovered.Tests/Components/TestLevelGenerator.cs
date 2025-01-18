using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Maps;
using MarsUndiscovered.Game.Components.Maps.MapPointChoiceRules;
using MarsUndiscovered.Game.Extensions;
using SadRogue.Primitives;

namespace MarsUndiscovered.Tests.Components
{
    public class TestLevelGenerator : ILevelGenerator
    {
        private GameWorld _gameWorld;

        private readonly ILevelGenerator _originalLevelGenerator;
        public IMapGenerator MapGenerator { get; set; }
        public IMonsterGenerator MonsterGenerator { get; set; }
        public IItemGenerator ItemGenerator { get; set; }
        public IShipGenerator ShipGenerator { get; set; }
        public IMapExitGenerator MapExitGenerator { get; set; }
        public IMachineGenerator MachineGenerator { get; set; }
   
        public Point OutdoorMapSize { get; set; }

        public Point PlayerPosition { get; set; } = Point.None;

        // Specify the counts here. For controlling the type of item to create, use
        // the generators.
        public int ItemsToCreateCount { get; set; }
        public int MonstersToCreateCount { get; set; }
        public int MachinesToCreateCount { get; set; }
        public bool AddExits { get; set; }

        public TestLevelGenerator(GameWorld gameWorld, IMapGenerator mapGenerator, int mapWidth, int mapHeight)
        {
            OutdoorMapSize = new Point(mapWidth, mapHeight);

            _gameWorld = gameWorld;
            _originalLevelGenerator = _gameWorld.LevelGenerator;

            MapGenerator = mapGenerator ?? new BlankMapGenerator(_gameWorld.GameObjectFactory);
            MonsterGenerator = _originalLevelGenerator.MonsterGenerator;
            ItemGenerator = _originalLevelGenerator.ItemGenerator;
            MapExitGenerator = _originalLevelGenerator.MapExitGenerator;
            MachineGenerator = _originalLevelGenerator.MachineGenerator;
            
            // Currently not used
            ShipGenerator = _originalLevelGenerator.ShipGenerator;
        }

        public void CreateLevels()
        {
            var map = CreateLevel1();
            CreateLevel2(map);
        }

        private MarsMap CreateLevel1()
        {
            MapGenerator.CreateOutdoorMap(_gameWorld, _gameWorld.GameObjectFactory, OutdoorMapSize.X, OutdoorMapSize.Y);
            _gameWorld.AddMapToGame(MapGenerator.Map);
            _gameWorld.Maps.CurrentMap = MapGenerator.Map;

            if (PlayerPosition == Point.None)
            {
                PlayerPosition = new Point(MapGenerator.Map.Width / 2,
                    MapGenerator.Map.Height - 2 -
                    (UiConstants.ShipOffset -
                     1)); // Start off underneath the ship, extra -1 for the current ship design as there's a blank space on the bottom line
            }

            _gameWorld.Player = _gameWorld.GameObjectFactory
                .CreateGameObject<Player>()
                .PositionedAt(PlayerPosition)
                .AddToMap(MapGenerator.Map);

            CreateMapExitToNextMap(MapGenerator.Map);
            
            for (var i = 0; i < MonstersToCreateCount; i++)
            {
                SpawnMonster(new SpawnMonsterParams { MapId = MapGenerator.Map.Id });
            }

            for (var i = 0; i < ItemsToCreateCount; i++)
            {
                SpawnItem(new SpawnItemParams { MapId = MapGenerator.Map.Id });
            }
            
            for (var i = 0; i < MachinesToCreateCount; i++)
            {
                SpawnMachine(new SpawnMachineParams { MapId = MapGenerator.Map.Id });
            }
            
            return MapGenerator.Map;
        }
        
        private void SpawnMachine(SpawnMachineParams spawnMachineParams)
        {
            spawnMachineParams.MachineType = MachineType.Analyzer;
            
            MachineGenerator.SpawnMachine(spawnMachineParams, _gameWorld.GameObjectFactory, _gameWorld.Maps, _gameWorld.Machines);
        }
    
        private void SpawnMonster(SpawnMonsterParams spawnMonsterParams)
        {
            MonsterGenerator.SpawnMonster(spawnMonsterParams, _gameWorld.GameObjectFactory, _gameWorld.Maps, _gameWorld.Monsters);
        }

        private void SpawnItem(SpawnItemParams spawnItemParams)
        {
            ItemGenerator.SpawnItem(spawnItemParams, _gameWorld.GameObjectFactory, _gameWorld.Maps, _gameWorld.Items);
        }

        private void CreateLevel2(MarsMap previousMap)
        {
            MapGenerator.CreateOutdoorMap(_gameWorld, _gameWorld.GameObjectFactory, OutdoorMapSize.X, OutdoorMapSize.Y);
            _gameWorld.AddMapToGame(MapGenerator.Map);
            CreateMapExitToPreviousMap(MapGenerator.Map, previousMap);
        }

        private MapExit SpawnMapExit(SpawnMapExitParams spawnMapExitParams)
        {
            spawnMapExitParams.MapPointChoiceRules.Add(new WallAdjacentToFloorRule());
            spawnMapExitParams.WithSeparationBetweenMapExitPoints();

            MapExitGenerator.SpawnMapExit(spawnMapExitParams, _gameWorld.GameObjectFactory, _gameWorld.Maps, _gameWorld.MapExits);
            return spawnMapExitParams.Result;
        }

        private void CreateMapExitToNextMap(MarsMap map)
        {
            if (!AddExits)
                return;
            
            SpawnMapExit(
                new SpawnMapExitParams()
                    .OnMap(map.Id)
                    .WithDirection(MapExitDirection.Down)
            );
        }

        private void CreateMapExitToPreviousMap(MarsMap currentMap, MarsMap previousMap)
        {
            if (!AddExits)
                return;
            
            var mapExit = SpawnMapExit(new SpawnMapExitParams()
                .OnMap(currentMap.Id)
                .WithDirection(MapExitDirection.Up));

            if (mapExit != null)
                LinkMapExit(previousMap, mapExit);
        }

        private void LinkMapExit(MarsMap previousMap, MapExit mapExit)
        {
            var previousMapExit = _gameWorld.MapExits.Values.First(me => Equals(me.CurrentMap, previousMap) && me.Direction == MapExitDirection.Down);
            mapExit.Destination = previousMapExit;
            previousMapExit.Destination = mapExit;
        }

        public void Initialise(GameWorld gameWorld)
        {
            _gameWorld = gameWorld;
            _originalLevelGenerator.Initialise(gameWorld);
        }

        public ProgressiveWorldGenerationResult CreateProgressive(ulong seed, int step,
            WorldGenerationTypeParams worldGenerationTypeParams)
        {
            _originalLevelGenerator.MapGenerator = MapGenerator;
            return _originalLevelGenerator.CreateProgressive(seed, step, worldGenerationTypeParams);
        }
    }
}