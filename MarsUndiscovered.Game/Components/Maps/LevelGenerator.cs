﻿using GoRogue.Random;
using MarsUndiscovered.Game.Extensions;
using SadRogue.Primitives;
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

    private MapExit SpawnMapExit(SpawnMapExitParams spawnMapExitParams)
    {
        return MapExitGenerator.SpawnMapExit(spawnMapExitParams, _gameWorld.GameObjectFactory, _gameWorld.Maps, _gameWorld.MapExits);
    }

    private MarsMap CreateLevel2(MarsMap mapLevel1)
    {
        MapGenerator.CreateOutdoorMap(_gameWorld, _gameWorld.GameObjectFactory);
        _gameWorld.AddMapToGame(MapGenerator.Map);
        var mapLevel2 = MapGenerator.Map;
        mapLevel2.Level = 2;

        SpawnMonster(new SpawnMonsterParams().OnMap(mapLevel2.Id).WithBreed("Roach"));
        SpawnMonster(new SpawnMonsterParams().OnMap(mapLevel2.Id).WithBreed("RepairDroid"));
        SpawnMonster(new SpawnMonsterParams().OnMap(mapLevel2.Id).WithBreed("TeslaTurret"));

        SpawnItem(new SpawnItemParams().OnMap(mapLevel2.Id).WithItemType(ItemType.MagnesiumPipe));
        SpawnItem(new SpawnItemParams().OnMap(mapLevel2.Id).WithItemType(ItemType.MagnesiumPipe));
        SpawnItem(new SpawnItemParams().OnMap(mapLevel2.Id).WithItemType(ItemType.IronSpike));
        SpawnItem(new SpawnItemParams().OnMap(mapLevel2.Id).WithItemType(ItemType.IronSpike));
        SpawnItem(new SpawnItemParams().OnMap(mapLevel2.Id).WithItemType(ItemType.ShieldGenerator));
        SpawnItem(new SpawnItemParams().OnMap(mapLevel2.Id).WithItemType(ItemType.ShieldGenerator));
        SpawnItem(new SpawnItemParams().OnMap(mapLevel2.Id).WithItemType(ItemType.HealingBots));
        SpawnItem(new SpawnItemParams().OnMap(mapLevel2.Id).WithItemType(ItemType.HealingBots));
        SpawnItem(new SpawnItemParams().OnMap(mapLevel2.Id).WithItemType(ItemType.ShipRepairParts));

        var mapExit2 = SpawnMapExit(new SpawnMapExitParams().OnMap(mapLevel2.Id).WithDirection(Direction.Up));

        if (mapExit2 != null)
        {
            var miningFacilityPointsOnMap1 = _gameWorld.MiningFacilities.Values
                .Where(m => ((MarsMap)m.CurrentMap).Id == mapLevel1.Id)
                .Select(m => m.Position)
                .GroupBy(m => m.Y)
                .MaxBy(m => m.Key)
                .ToList();
            
            var mapExit1 = SpawnMapExit(
                new SpawnMapExitParams()
                    .OnMap(mapLevel1.Id)
                    .ToMapExit(mapExit2.ID)
                    .WithDirection(Direction.Down)
                    .AtFreeSpotNextTo(mapLevel1, miningFacilityPointsOnMap1)
            );

            mapExit2.Destination = mapExit1;
        }

        return mapLevel2;
    }

    private MarsMap CreateLevel1()
    {
        MapGenerator.CreateOutdoorMap(_gameWorld, _gameWorld.GameObjectFactory);
        _gameWorld.AddMapToGame(MapGenerator.Map);
        var map = MapGenerator.Map;
        
        ShipGenerator.CreateShip(_gameWorld.GameObjectFactory, map, _gameWorld.Ships);
        MiningFacilityGenerator.CreateMiningFacility(_gameWorld.GameObjectFactory, map, _gameWorld.MiningFacilities);
        
        _gameWorld.Player = _gameWorld.GameObjectFactory
            .CreateGameObject<Player>()
            .PositionedAt(new Point(map.Width / 2,
                map.Height - 2 -
                (Constants.ShipOffset -
                 1))) // Start off underneath the ship, extra -1 for the current ship design as there's a blank space on the bottom line
            .AddToMap(map);

        SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Black Ops Defender")).OnMap(map.Id));
        SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Black Ops Sniper")).OnMap(map.Id));
        SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Crazed Foreman")).OnMap(map.Id));
        SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Crazed Miner")).OnMap(map.Id));
        SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Eldritch Worm")).OnMap(map.Id));
        SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Flame Turret")).OnMap(map.Id));
        SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Megalith Carrier")).OnMap(map.Id));
        SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Megalith Chucker")).OnMap(map.Id));
        SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Missile Turret")).OnMap(map.Id));
        SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Nirgal Bomber")).OnMap(map.Id));
        SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Nirgal Rebel")).OnMap(map.Id));
        SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Phasmid Hunter")).OnMap(map.Id));
        SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Repair Droid")).OnMap(map.Id));
        SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Roach")).OnMap(map.Id));
        SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Spiral Borer")).OnMap(map.Id));
        SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Steam Elemental")).OnMap(map.Id));
        SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Tentacle Horror")).OnMap(map.Id));
        SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Tesla Turret")).OnMap(map.Id));
        SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Tunnel Borer")).OnMap(map.Id));
        SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Werewolf")).OnMap(map.Id));
        SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Yendorian Grunt")).OnMap(map.Id));
        SpawnMonster(new SpawnMonsterParams().WithBreed(Breed.GetBreed("Yendorian Master")).OnMap(map.Id));
        
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

    public void CreateLevels()
    {
        var level1Map = CreateLevel1();
        CreateLevel2(level1Map);
    }

    public ProgressiveWorldGenerationResult CreateProgressive(ulong seed, int step, WorldGenerationTypeParams worldGenerationTypeParams)
    {
        switch (worldGenerationTypeParams.MapType)
        {
            case MapType.Outdoor:
                MapGenerator.CreateOutdoorMap(_gameWorld, _gameWorld.GameObjectFactory, step);
                break;
            case MapType.Mine:
                MapGenerator.CreateMineMap(_gameWorld, _gameWorld.GameObjectFactory, step);
                break;
        }

        _gameWorld.AddMapToGame(MapGenerator.Map);

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