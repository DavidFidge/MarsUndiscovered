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
    
    private ProbabilityTable<MachineType> _machineTypeProbabilityTable;
    private List<(MachineType machineType, double weight)> _machineTypeWeights;

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
    public IMachineGenerator MachineGenerator { get; set; }
    public IFeatureGenerator FeatureGenerator { get; set; }
    
    public IEnhancedRandom RNG { get; set; }
    
    private void SpawnMachine(SpawnMachineParams spawnMachineParams)
    {
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
    
    private void SpawnFeature(SpawnFeatureParams spawnFeatureParams)
    {
        FeatureGenerator.SpawnFeature(spawnFeatureParams, _gameWorld.GameObjectFactory, _gameWorld.Maps, _gameWorld.Features);
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
                .WithDirection(MapExitDirection.Down)
        );
    }

    private void CreateMapExitToPreviousMap(MarsMap currentMap, MarsMap previousMap)
    {
        var spawnMapExitParams = new SpawnMapExitParams()
            .OnMap(currentMap.Id)
            .WithDirection(MapExitDirection.Up);
        
        SpawnMapExit(spawnMapExitParams);

        LinkMapExit(previousMap, spawnMapExitParams.Result);
    }

    private void LinkMapExit(MarsMap previousMap, MapExit mapExit)
    {
        var previousMapExit = _gameWorld.MapExits.Values.First(me => Equals(me.CurrentMap, previousMap) && me.Direction == MapExitDirection.Down);
        mapExit.Destination = previousMapExit;
        previousMapExit.Destination = mapExit;
    }

    private void SpawnItems(int itemsToPlace, MarsMap map)
    {
        for (var i = 0; i < itemsToPlace; i++)
        {
            var itemType = _itemTypeProbabilityTable.NextItem().NextItem();

            var spawnItemParams = new SpawnItemParams()
                .OnMap(map.Id)
                .WithItemType(itemType)
                .WithRandomEnchantmentLevel();

            SpawnItem(spawnItemParams);
        }
    }
    
    private void SpawnMachines(int machinesToPlace, MarsMap map)
    {
        for (var i = 0; i < machinesToPlace; i++)
        {
            var machineItem = _machineTypeProbabilityTable.NextItem();

            var spawnMachineParams = new SpawnMachineParams()
                .OnMap(map.Id)
                .WithMachineType(machineItem);

            SpawnMachine(spawnMachineParams);
        }
    }
    
    private MarsMap CreateLevel1()
    {
        MapGenerator.CreateOutdoorMap(_gameWorld, _gameWorld.GameObjectFactory, 70, 70);
        
        _gameWorld.AddMapToGame(MapGenerator.Map);
        _gameWorld.Maps.CurrentMap = MapGenerator.Map;
        var map = MapGenerator.Map;

        ShipGenerator.CreateShip(_gameWorld.GameObjectFactory, map, _gameWorld.Ships);
        MiningFacilityGenerator.CreateMiningFacility(_gameWorld.GameObjectFactory, map, _gameWorld.MiningFacilities);


        for (var i = 0; i < 100; i++)
        {
            var rubbleParams = new SpawnFeatureParams()
                .WithFeatureType(FeatureType.RubbleType)
                .OnMap(map.Id);
            
            SpawnFeature(rubbleParams);
        }
        
        var pointsNextToBottomOfMiningFacility = _gameWorld.MiningFacilities.Values
            .Where(m => ((MarsMap)m.CurrentMap).Id == map.Id)
            .Select(m => m.Position)
            .GroupBy(m => m.Y)
            .MaxBy(m => m.Key)
            .Select(m => new Point(m.X, m.Y + 1))
            .ToList();

        var spawnMapExitParams = new SpawnMapExitParams()
            .OnMap(map.Id)
            .WithDirection(MapExitDirection.Down);
        
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
        
        var machinesToPlace = RNG.NextInt(0, 2);
        SpawnMachines(machinesToPlace, map);
        
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
        
        var machinesToPlace = RNG.NextInt(0, 2);
        SpawnMachines(machinesToPlace, map);
        
        return map;
    }

    private MarsMap CreateLevel3(MarsMap previousMap)
    {
        MapGenerator.CreateMineMap(_gameWorld, _gameWorld.GameObjectFactory, 60, 60);
        _gameWorld.AddMapToGame(MapGenerator.Map);
        var map = MapGenerator.Map;
        map.Level = 3;
        
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

        for (var i = 0; i < 5; i++)
            probabilityTable.NextItem().Spawn(map);
        
        var itemsToPlace = RNG.NextInt(5, 10);
        SpawnItems(itemsToPlace, map);
        
        var machinesToPlace = RNG.NextInt(0, 2);
        SpawnMachines(machinesToPlace, map);

        return map;
    }
    
    private MarsMap CreateLevel4(MarsMap previousMap)
    {
        MapGenerator.CreatePrefabMap(_gameWorld, _gameWorld.GameObjectFactory, 60, 60);
        _gameWorld.AddMapToGame(MapGenerator.Map);
        var map = MapGenerator.Map;
        map.Level = 4;

        SpawnItem(new SpawnItemParams().OnMap(map.Id).WithItemType(ItemType.ShipRepairParts));
        SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Yendorian Master")).OnMap(map.Id));
        
        CreateMapExitToPreviousMap(map, previousMap);
        
        var itemsToPlace = RNG.NextInt(5, 10);
        SpawnItems(itemsToPlace, map);
        
        var machinesToPlace = RNG.NextInt(0, 2);
        SpawnMachines(machinesToPlace, map);

        return map;
    }

    public void CreateLevels()
    {
        SetupItemWeightTable();
        SetupMachineWeightTable();
        
        var level1Map = CreateLevel1();
        var level2Map = CreateLevel2(level1Map);
        var level3Map = CreateLevel3(level2Map);
        
        CreateLevel4(level3Map);
    }

    private void SetupItemWeightTable()
    {
        _gadgetWeights = new List<(ItemType itemType, double weight)>
        {
            (ItemType.ShieldGenerator, 1), 
            (ItemType.ForcePush, 1)
        };

        _gadgetProbabilityTable = new ProbabilityTable<ItemType>(_gadgetWeights);
        _gadgetProbabilityTable.Random = RNG;
        
        _weaponWeights = new List<(ItemType itemType, double weight)>
        {
            (ItemType.IronSpike, 1), 
            (ItemType.MagnesiumPipe, 1), 
            (ItemType.LaserPistol, 10) 
        };

        _weaponProbabilityTable = new ProbabilityTable<ItemType>(_weaponWeights);
        _weaponProbabilityTable.Random = RNG;
        
        _nanoFlaskWeights = new List<(ItemType itemType, double weight)>
        {
            (ItemType.HealingBots, 1), 
            (ItemType.EnhancementBots, 1) 
        };

        _nanoFlaskProbabilityTable = new ProbabilityTable<ItemType>(_nanoFlaskWeights);
        _nanoFlaskProbabilityTable.Random = RNG;
        
        _itemTypeWeights = new List<(ProbabilityTable<ItemType> itemType, double weight)>
        {
            (_gadgetProbabilityTable, 1), 
            (_weaponProbabilityTable, 1), 
            (_nanoFlaskProbabilityTable, 1)
        };
        
        _itemTypeProbabilityTable = new ProbabilityTable<ProbabilityTable<ItemType>>(_itemTypeWeights);
        _itemTypeProbabilityTable.Random = RNG;
    }
    
    private void SetupMachineWeightTable()
    {
        _machineTypeWeights = new List<(MachineType machineType, double weight)>
        {
            (MachineType.Analyzer, 1)
        };
        
        _machineTypeProbabilityTable = new ProbabilityTable<MachineType>(_machineTypeWeights);
        _machineTypeProbabilityTable.Random = RNG;
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
            case MapType.Prefab:
                MapGenerator.CreatePrefabMap(_gameWorld, _gameWorld.GameObjectFactory, 60, 60, step);
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