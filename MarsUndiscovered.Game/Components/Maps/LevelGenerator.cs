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
    private List<(ProbabilityTable<ItemType> itemType, double weight)> _itemTypeWeights;
    private ProbabilityTable<ProbabilityTable<ItemType>> _itemTypeProbabilityTable;
    private ProbabilityTable<ItemType> _gadgetProbabilityTable;
    private List<(ItemType itemType, double weight)> _gadgetWeights;
    private List<(ItemType itemType, double weight)> _weaponWeights;
    private ProbabilityTable<ItemType> _weaponProbabilityTable;
    private List<(ItemType itemType, double weight)> _nanoFlaskWeights;
    private ProbabilityTable<ItemType> _nanoFlaskProbabilityTable;
    public IMapGenerator MapGenerator { get; set; }
    public IMonsterGenerator MonsterGenerator { get; set; }
    public IItemGenerator ItemGenerator { get; set; }
    public IShipGenerator ShipGenerator { get; set; }
    public IMiningFacilityGenerator MiningFacilityGenerator { get; set; }
    public IMapExitGenerator MapExitGenerator { get; set; }

    public IEnhancedRandom RNG { get; set; }
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
        spawnMapExitParams.WithSeparationBetweenMapExitPoints();
        
        MapExitGenerator.SpawnMapExit(spawnMapExitParams, _gameWorld.GameObjectFactory, _gameWorld.Maps, _gameWorld.MapExits);
    }
    
    private void SpawnMapExitWithoutDefaultRules(SpawnMapExitParams spawnMapExitParams)
    {
        spawnMapExitParams.MapPointChoiceRules.Add(new EmptyFloorRule());
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

        var pointsNextToBottomOfMiningFacility = _gameWorld.MiningFacilities.Values
            .Where(m => ((MarsMap)m.CurrentMap).Id == map.Id)
            .Select(m => m.Position)
            .GroupBy(m => m.Y)
            .MaxBy(m => m.Key)
            .Select(m => new Point(m.X, m.Y + 1))
            .ToList();

        var spawnMapExitParams = new SpawnMapExitParams()
            .OnMap(map.Id)
            .WithDirection(Direction.Down);
        
        spawnMapExitParams.MapPointChoiceRules.Add(new RestrictedSetRule(pointsNextToBottomOfMiningFacility));
        SpawnMapExitWithoutDefaultRules(spawnMapExitParams);

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
                (new SingleMonsterSpawner(MonsterGenerator, _gameWorld, Breed.GetBreed("Rat")), 1),
                (new SingleMonsterSpawner(MonsterGenerator, _gameWorld, Breed.GetBreed("Roach")), 1),
                (new SingleMonsterSpawner(MonsterGenerator, _gameWorld, Breed.GetBreed("Blood Fly")), 1),
                (new VariableCountMonsterSpawner(MonsterGenerator, _gameWorld, Breed.GetBreed("Repair Droid"), RNG, 2, 4), 1)
            }
        );
            
        probabilityTable.Random = RNG;

        for (var i = 0; i < 10; i++)
            probabilityTable.NextItem().Spawn(map);

        var itemsToPlace = RNG.NextInt(5, 10);
        SpawnItems(itemsToPlace, map);
        
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
        
        return map;
    }

    private void SpawnItems(int itemsToPlace, MarsMap map)
    {
        for (var i = 0; i < itemsToPlace; i++)
        {
            var itemType = _itemTypeProbabilityTable.NextItem().NextItem();

            var spawnItemParams = new SpawnItemParams()
                .OnMap(map.Id)
                .WithItemType(itemType);

            SpawnItem(spawnItemParams);
        }
    }

    private MarsMap CreateLevel2(MarsMap previousMap)
    {
        MapGenerator.CreateMiningFacilityMap(_gameWorld, _gameWorld.GameObjectFactory, 60, 60);
        _gameWorld.AddMapToGame(MapGenerator.Map);
        var map = MapGenerator.Map;
        map.Level = 2;

        CreateMapExitToPreviousMap(map, previousMap);
        CreateMapExitToNextMap(map);

        var probabilityTable = new ProbabilityTable<MonsterSpawner>(
            new List<(MonsterSpawner monsterSpawner, double weight)>
            {
                (new SingleMonsterSpawner(MonsterGenerator, _gameWorld, Breed.GetBreed("Rat")), 1),
                (new SingleMonsterSpawner(MonsterGenerator, _gameWorld, Breed.GetBreed("Roach")), 1),
                (new SingleMonsterSpawner(MonsterGenerator, _gameWorld, Breed.GetBreed("Blood Fly")), 1),
                (new VariableCountMonsterSpawner(MonsterGenerator, _gameWorld, Breed.GetBreed("Repair Droid"), RNG, 2, 4), 1),
                (new SingleMonsterSpawner(MonsterGenerator, _gameWorld, Breed.GetBreed("Cleaning Droid")), 1)
            }
        );
            
        probabilityTable.Random = RNG;

        for (var i = 0; i < 10; i++)
            probabilityTable.NextItem().Spawn(map);

        var itemsToPlace = RNG.NextInt(5, 10);
        SpawnItems(itemsToPlace, map);

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
        
        var itemsToPlace = RNG.NextInt(5, 10);
        SpawnItems(itemsToPlace, map);
        
        return map;
    }

    public void CreateLevels()
    {
        SetupItemWeightTable();
        var level1Map = CreateLevel1();
        var level2Map = CreateLevel2(level1Map);
        CreateLevel3(level2Map);
    }

    private void SetupItemWeightTable()
    {
        _gadgetWeights = new List<(ItemType itemType, double weight)>
        {
            (ItemType.ShieldGenerator, 1) 
        };

        _gadgetProbabilityTable = new ProbabilityTable<ItemType>(_gadgetWeights);

        _weaponWeights = new List<(ItemType itemType, double weight)>
        {
            (ItemType.IronSpike, 1), 
            (ItemType.MagnesiumPipe, 1) 
        };

        _weaponProbabilityTable = new ProbabilityTable<ItemType>(_weaponWeights);

        _nanoFlaskWeights = new List<(ItemType itemType, double weight)>
        {
            (ItemType.HealingBots, 1), 
            (ItemType.EnhancementBots, 1) 
        };

        _nanoFlaskProbabilityTable = new ProbabilityTable<ItemType>(_nanoFlaskWeights);
        
        _itemTypeWeights = new List<(ProbabilityTable<ItemType> itemType, double weight)>
        {
            (_gadgetProbabilityTable, 1), 
            (_weaponProbabilityTable, 1), 
            (_nanoFlaskProbabilityTable, 1)
        };
        
        _itemTypeProbabilityTable = new ProbabilityTable<ProbabilityTable<ItemType>>(_itemTypeWeights);
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
            .PositionedAt(RNG.RandomPosition(MapGenerator.Map, MapHelpers.EmptyPointOnFloor))
            .AddToMap(MapGenerator.Map);

        return new ProgressiveWorldGenerationResult { Seed = seed, IsFinalStep = true };
    }

    public void Initialise(GameWorld gameWorld)
    {
        _gameWorld = gameWorld;
        RNG = GlobalRandom.DefaultRNG;
    }
}