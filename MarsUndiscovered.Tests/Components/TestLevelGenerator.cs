using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Maps;
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
        public IMiningFacilityGenerator MiningFacilityGenerator { get; set; }
        public IMapExitGenerator MapExitGenerator { get; set; }

        public Point OutdoorMapSize { get; set; }

        public TestLevelGenerator(GameWorld gameWorld, IMapGenerator mapGenerator, int mapWidth, int mapHeight)
        {
            OutdoorMapSize = new Point(mapWidth, mapHeight);

            _gameWorld = gameWorld;
            _originalLevelGenerator = _gameWorld.LevelGenerator;

            MapGenerator = mapGenerator ?? new BlankMapGenerator(_gameWorld.GameObjectFactory);
            MonsterGenerator = _originalLevelGenerator.MonsterGenerator;
            ItemGenerator = _originalLevelGenerator.ItemGenerator;
            ShipGenerator = _originalLevelGenerator.ShipGenerator;
            MiningFacilityGenerator = _originalLevelGenerator.MiningFacilityGenerator;
            MapExitGenerator = _originalLevelGenerator.MapExitGenerator;
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

            _gameWorld.Player = _gameWorld.GameObjectFactory
                .CreateGameObject<Player>()
                .PositionedAt(new Point(MapGenerator.Map.Width / 2,
                    MapGenerator.Map.Height - 2 -
                    (Constants.ShipOffset -
                     1))) // Start off underneath the ship, extra -1 for the current ship design as there's a blank space on the bottom line
                .AddToMap(MapGenerator.Map);

            CreateMapExitToNextMap(MapGenerator.Map);

            return MapGenerator.Map;
        }

        private void CreateLevel2(MarsMap previousMap)
        {
            MapGenerator.CreateOutdoorMap(_gameWorld, _gameWorld.GameObjectFactory, OutdoorMapSize.X, OutdoorMapSize.Y);
            _gameWorld.AddMapToGame(MapGenerator.Map);
            CreateMapExitToPreviousMap(MapGenerator.Map, previousMap);
        }

        private MapExit SpawnMapExit(SpawnMapExitParams spawnMapExitParams)
        {
            return MapExitGenerator.SpawnMapExit(spawnMapExitParams, _gameWorld.GameObjectFactory, _gameWorld.Maps, _gameWorld.MapExits);
        }

        private void CreateMapExitToNextMap(MarsMap map)
        {
            SpawnMapExit(
                new SpawnMapExitParams()
                    .OnMap(map.Id)
                    .WithDirection(Direction.Down)
            );
        }

        private void CreateMapExitToPreviousMap(MarsMap currentMap, MarsMap previousMap)
        {
            var mapExit = SpawnMapExit(new SpawnMapExitParams()
                .OnMap(currentMap.Id)
                .WithDirection(Direction.Up));

            if (mapExit != null)
                LinkMapExit(previousMap, mapExit);
        }

        private void LinkMapExit(MarsMap previousMap, MapExit mapExit)
        {
            var previousMapExit = _gameWorld.MapExits.Values.First(me => Equals(me.CurrentMap, previousMap) && me.Direction == Direction.Down);
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