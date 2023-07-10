using FrigidRogue.MonoGame.Core.Components.MapPointChoiceRules;
using GoRogue.Random;
using MarsUndiscovered.Game.Components.Maps.MapPointChoiceRules;
using MarsUndiscovered.Game.Extensions;
using SadRogue.Primitives;
using ShaiRandom.Collections;
using ShaiRandom.Generators;

namespace MarsUndiscovered.Game.Components.Maps;

public class LevelGenerator : ILevelGenerator
{
    private GameWorld _gameWorld;
    public IMapGenerator MapGenerator { get; set; }
    public IMonsterGenerator MonsterGenerator { get; set; }
    public IItemGenerator ItemGenerator { get; set; }
    public IShipGenerator ShipGenerator { get; set; }
    public IMiningFacilityGenerator MiningFacilityGenerator { get; set; }
    public IMapExitGenerator MapExitGenerator { get; set; }
    
    private void SpawnMonster(SpawnMonsterParams spawnMonsterParams)
    {
        MonsterGenerator.SpawnMonster(spawnMonsterParams, _gameWorld.GameObjectFactory, _gameWorld.Maps, _gameWorld.Monsters);
    }

    private void SpawnItem(SpawnItemParams spawnItemParams)
    {
        ItemGenerator.SpawnItem(spawnItemParams, _gameWorld.GameObjectFactory, _gameWorld.Maps, _gameWorld.Items);
    }

    private void SpawnMapExit(SpawnMapExitParams spawnMapExitParams)
    {
        spawnMapExitParams.MapPointChoiceRules.Add(new WallAdjacentToFloorRule());
        MapExitGenerator.SpawnMapExit(spawnMapExitParams, _gameWorld.GameObjectFactory, _gameWorld.Maps, _gameWorld.MapExits);
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
        var spawnMapExitParams = new SpawnMapExitParams()
            .OnMap(currentMap.Id)
            .WithDirection(Direction.Up);
        
        SpawnMapExit(spawnMapExitParams);

        LinkMapExit(previousMap, spawnMapExitParams.Result);
    }

    private void LinkMapExit(MarsMap previousMap, MapExit mapExit)
    {
        var previousMapExit = _gameWorld.MapExits.Values.First(me => Equals(me.CurrentMap, previousMap) && me.Direction == Direction.Down);
        mapExit.Destination = previousMapExit;
        previousMapExit.Destination = mapExit;
    }

    private MarsMap CreateLevel1()
    {
        MapGenerator.CreateOutdoorMap(_gameWorld, _gameWorld.GameObjectFactory, 70, 70);
        
        _gameWorld.AddMapToGame(MapGenerator.Map);
        _gameWorld.Maps.CurrentMap = MapGenerator.Map;
        var map = MapGenerator.Map;

        ShipGenerator.CreateShip(_gameWorld.GameObjectFactory, map, _gameWorld.Ships);
        MiningFacilityGenerator.CreateMiningFacility(_gameWorld.GameObjectFactory, map, _gameWorld.MiningFacilities);

        var miningFacilityPoints = _gameWorld.MiningFacilities.Values
            .Where(m => ((MarsMap)m.CurrentMap).Id == map.Id)
            .Select(m => m.Position)
            .GroupBy(m => m.Y)
            .MaxBy(m => m.Key)
            .ToList();

        var spawnMapExitParams = new SpawnMapExitParams()
            .OnMap(map.Id)
            .WithDirection(Direction.Down);
        
        spawnMapExitParams.MapPointChoiceRules.Add(new RestrictedSetRule(miningFacilityPoints));
        SpawnMapExit(spawnMapExitParams);

        _gameWorld.Player = _gameWorld.GameObjectFactory
            .CreateGameObject<Player>()
            .PositionedAt(new Point(map.Width / 2,
                map.Height - 2 -
                (Constants.ShipOffset -
                 1))) // Start off underneath the ship, extra -1 for the current ship design as there's a blank space on the bottom line
            .AddToMap(map);

        var probabilityTable = new ProbabilityTable<MonsterSpawner>(
            new List<(MonsterSpawner monsterSpawner, double weight)>
            {
                (new SingleMonsterSpawner(MonsterGenerator, _gameWorld, Breed.GetBreed("Roach")), 1),
                (new VariableCountMonsterSpawner(MonsterGenerator, _gameWorld, Breed.GetBreed("RepairDroid"), GlobalRandom.DefaultRNG, 2, 4), 1)
            }
        );
            
        probabilityTable.Random = GlobalRandom.DefaultRNG;
        
        for (var i = 0; i < 10; i++)
            probabilityTable.NextItem().Spawn(map);
        
        // SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Black Ops Defender")).OnMap(map.Id));
        // SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Black Ops Sniper")).OnMap(map.Id));
        // SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Crazed Foreman")).OnMap(map.Id));
        // SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Crazed Miner")).OnMap(map.Id));
        // SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Eldritch Worm")).OnMap(map.Id));
        // SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Flame Turret")).OnMap(map.Id));
        // SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Megalith Carrier")).OnMap(map.Id));
        // SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Megalith Chucker")).OnMap(map.Id));
        // SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Missile Turret")).OnMap(map.Id));
        // SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Nirgal Bomber")).OnMap(map.Id));
        // SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Nirgal Rebel")).OnMap(map.Id));
        // SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Phasmid Hunter")).OnMap(map.Id));
        // SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Repair Droid")).OnMap(map.Id));
        // SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
        // SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Spiral Borer")).OnMap(map.Id));
        // SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Steam Elemental")).OnMap(map.Id));
        // SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Tentacle Horror")).OnMap(map.Id));
        // SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Tesla Turret")).OnMap(map.Id));
        // SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Tunnel Borer")).OnMap(map.Id));
        // SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Werewolf")).OnMap(map.Id));
        // SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Yendorian Grunt")).OnMap(map.Id));
        // SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Yendorian Master")).OnMap(map.Id));
        
        SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe).OnMap(map.Id));
        SpawnItem(new SpawnItemParams().WithItemType(ItemType.MagnesiumPipe).OnMap(map.Id));
        SpawnItem(new SpawnItemParams().WithItemType(ItemType.IronSpike).OnMap(map.Id));
        SpawnItem(new SpawnItemParams().WithItemType(ItemType.IronSpike).OnMap(map.Id));
        SpawnItem(new SpawnItemParams().WithItemType(ItemType.IronSpike).OnMap(map.Id));
        SpawnItem(new SpawnItemParams().WithItemType(ItemType.IronSpike).OnMap(map.Id));
        SpawnItem(new SpawnItemParams().WithItemType(ItemType.ShieldGenerator).OnMap(map.Id));
        SpawnItem(new SpawnItemParams().WithItemType(ItemType.ShieldGenerator).OnMap(map.Id));
        SpawnItem(new SpawnItemParams().WithItemType(ItemType.HealingBots).OnMap(map.Id));
        SpawnItem(new SpawnItemParams().WithItemType(ItemType.HealingBots).OnMap(map.Id));

        return map;
    }

    private MarsMap CreateLevel2(MarsMap previousMap)
    {
        MapGenerator.CreateMiningFacilityMap(_gameWorld, _gameWorld.GameObjectFactory, 60, 60);
        _gameWorld.AddMapToGame(MapGenerator.Map);
        var map = MapGenerator.Map;
        map.Level = 2;

        CreateMapExitToPreviousMap(map, previousMap);
        CreateMapExitToNextMap(map);

        return map;
    }

    private MarsMap CreateLevel3(MarsMap previousMap)
    {
        MapGenerator.CreateMineMap(_gameWorld, _gameWorld.GameObjectFactory, 60, 60);
        _gameWorld.AddMapToGame(MapGenerator.Map);
        var map = MapGenerator.Map;
        map.Level = 3;

        SpawnItem(new SpawnItemParams().OnMap(map.Id).WithItemType(ItemType.ShipRepairParts));

        CreateMapExitToPreviousMap(map, previousMap);

        return map;
    }

    public void CreateLevels()
    {
        var level1Map = CreateLevel1();
        var level2Map = CreateLevel2(level1Map);
        CreateLevel3(level2Map);
    }

    public ProgressiveWorldGenerationResult CreateProgressive(ulong seed, int step, WorldGenerationTypeParams worldGenerationTypeParams)
    {
        switch (worldGenerationTypeParams.MapType)
        {
            case MapType.Outdoor:
                MapGenerator.CreateOutdoorMap(_gameWorld, _gameWorld.GameObjectFactory, 70, 70, step);
                break;
            case MapType.Mine:
                MapGenerator.CreateMineMap(_gameWorld, _gameWorld.GameObjectFactory, 60, 60, step);
                break;
            case MapType.MiningFacility:
                MapGenerator.CreateMiningFacilityMap(_gameWorld, _gameWorld.GameObjectFactory, 60, 60, step);
                break;
        }

        _gameWorld.AddMapToGame(MapGenerator.Map);
        _gameWorld.Maps.CurrentMap = MapGenerator.Map;

        if (!MapGenerator.IsComplete || step <= MapGenerator.Steps)
            return new ProgressiveWorldGenerationResult { Seed = seed, IsFinalStep = false};

        _gameWorld.Player = _gameWorld.GameObjectFactory
            .CreateGameObject<Player>()
            .PositionedAt(GlobalRandom.DefaultRNG.RandomPosition(MapGenerator.Map, MapHelpers.EmptyPointOnFloor))
            .AddToMap(MapGenerator.Map);

        return new ProgressiveWorldGenerationResult { Seed = seed, IsFinalStep = true };
    }

    public void Initialise(GameWorld gameWorld)
    {
        _gameWorld = gameWorld;
    }
}