using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Extensions;
using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Components.Maps;

public class LevelGenerator
{
    private readonly IMapGenerator _mapGenerator;
    GameWorld GameWorld { get; set; }
    public IMapGenerator MapGenerator { get; set; }
    public IMonsterGenerator MonsterGenerator { get; set; }
    public IItemGenerator ItemGenerator { get; set; }
    public IShipGenerator ShipGenerator { get; set; }
    public IMiningFacilityGenerator MiningFacilityGenerator { get; set; }
    public IMapExitGenerator MapExitGenerator { get; set; }
    public IGameObjectFactory GameObjectFactory { get; set; }

    public LevelGenerator(GameWorld gameWorld)
    {
        GameWorld = gameWorld;

        _mapGenerator = gameWorld.MapGenerator;
        MonsterGenerator = gameWorld.MonsterGenerator;
        ItemGenerator = gameWorld.ItemGenerator;
        MapExitGenerator = gameWorld.MapExitGenerator;
        ShipGenerator = gameWorld.ShipGenerator;
        MiningFacilityGenerator = gameWorld.MiningFacilityGenerator;
        MapGenerator = gameWorld.MapGenerator;
        GameObjectFactory = gameWorld.GameObjectFactory;
    }

    public void SpawnMonster(SpawnMonsterParams spawnMonsterParams)
    {
        var map = spawnMonsterParams.MapId.HasValue ? GameWorld.Maps.First(m => m.Id == spawnMonsterParams.MapId) : _mapGenerator.MarsMap;

        MonsterGenerator.SpawnMonster(spawnMonsterParams, GameObjectFactory, map, GameWorld.Monsters);
    }

    public void SpawnItem(SpawnItemParams spawnItemParams)
    {
        var map = spawnItemParams.MapId.HasValue ? GameWorld.Maps.First(m => m.Id == spawnItemParams.MapId) : _mapGenerator.MarsMap;
            
        if (spawnItemParams.IntoPlayerInventory)
            spawnItemParams.Inventory = GameWorld.Inventory;

        ItemGenerator.SpawnItem(spawnItemParams, GameObjectFactory, map, GameWorld.Items);
    }

    public MapExit SpawnMapExit(SpawnMapExitParams spawnMapExitParams)
    {
        var map = spawnMapExitParams.MapId.HasValue ? GameWorld.Maps.First(m => m.Id == spawnMapExitParams.MapId) : _mapGenerator.MarsMap;

        return MapExitGenerator.SpawnMapExit(spawnMapExitParams, GameObjectFactory, map, GameWorld.MapExits);
    }

    public MarsMap CreateLevel2(MarsMap mapLevel1)
    {
        _mapGenerator.CreateOutdoorMap(GameWorld, GameWorld.GameObjectFactory);
        GameWorld.AddMapToGame(_mapGenerator.MarsMap);
        var mapLevel2 = _mapGenerator.MarsMap;
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
            var miningFacilityPointsOnMap1 = GameWorld.MiningFacilities.Values
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

    public MarsMap CreateLevel1()
    {
        MapGenerator.CreateOutdoorMap(GameWorld, GameObjectFactory);
        GameWorld.AddMapToGame(MapGenerator.MarsMap);
        var map = MapGenerator.MarsMap;
        
        ShipGenerator.CreateShip(GameObjectFactory, map, GameWorld.Ships);
        MiningFacilityGenerator.CreateMiningFacility(GameObjectFactory, map, GameWorld.MiningFacilities);
        
        GameWorld.Player = GameObjectFactory
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
}