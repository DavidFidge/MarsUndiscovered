using System.Linq;

using AutoMapper;

using MarsUndiscovered.Commands;
using MarsUndiscovered.Components;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Components.SaveData;

namespace MarsUndiscovered.Installers
{
    public class SaveDataProfile : Profile
    {
        private readonly IGameObjectFactory _gameObjectFactory;

        public SaveDataProfile(IGameObjectFactory gameObjectFactory)
        {
            _gameObjectFactory = gameObjectFactory;
            MapForSave();
            MapForLoad();
        }

        private void MapForLoad()
        {



            ////CreateMap<ShipSaveData, Ship>()
            ////    .ConstructUsing(sd => _gameObjectFactory.CreateShip(sd.Id));

            ////CreateMap<InventorySaveData, Inventory>()
            ////    .ForMember(d => d.Items, o => o.Ignore())
            ////    .ForMember(d => d.ItemKeyAssignments, o => o.Ignore())
            ////    .ForMember(d => d.CallItem, o => o.Ignore())
            ////    .ForMember(d => d.CallItemType, o => o.Ignore())
            ////    .ForMember(d => d.ItemTypeDiscoveries, o => o.Ignore());

            ////CreateMap<GameWorldSaveData, GameWorld>();

            ////CreateMap<MapCollectionSaveData, MapCollection>()
            ////    .ForMember(d => d.CurrentMap, o => o.Ignore());

            ////CreateMap<MapSaveData, MarsMap>()
            ////    .ForMember(d => d.SeenTiles, o => o.Ignore());

            ////CreateMap<SeenTileSaveData, SeenTile>()
            ////    .ForMember(d => d.LastSeenGameObjects, o => o.Ignore());

            ////CreateMap<MoveCommandSaveData, MoveCommand>();
            ////CreateMap<WalkCommandSaveData, WalkCommand>();
            ////CreateMap<AttackCommandSaveData, AttackCommand>();
            ////CreateMap<DeathCommandSaveData, DeathCommand>();
            ////CreateMap<PickUpItemSaveData, PickUpItemCommand>();
            ////CreateMap<EquipItemSaveData, EquipItemCommand>();
            ////CreateMap<UnequipItemSaveData, UnequipItemCommand>();
            ////CreateMap<DropItemSaveData, DropItemCommand>();
        }

        private void MapForSave()
        {

            ////CreateMap<Item, ItemSaveData>();
            ////CreateMap<MapExit, MapExitSaveData>();
            ////CreateMap<GameWorld, GameWorldSaveData>();
            ////CreateMap<GameObjectFactory, GameObjectFactorySaveData>();

            ////CreateMap<Inventory, InventorySaveData>()
            ////    .ForMember(d => d.ItemIds, o => o.MapFrom(s => s.Items.Select(i => i.ID).ToList()))
            ////    .ForMember(d => d.ItemKeyAssignments, o => o.MapFrom(s => s.ItemKeyAssignments.ToDictionary(k => k.Key, v => v.Value.Select(i => i.ID).ToList())))
            ////    .ForMember(d => d.CallItem, o => o.MapFrom(s => s.CallItem.ToDictionary(k => k.Key.ID, v => v.Value)))
            ////    .ForMember(d => d.CallItemType, o => o.MapFrom(s => s.CallItemType.ToDictionary(k => k.Key.Name, v => v.Value)))
            ////    .ForMember(d => d.ItemTypeDiscoveries, o => o.MapFrom(s => s.ItemTypeDiscoveries.ToDictionary(k => k.Key.Name, v => v.Value)));

            ////CreateMap<MoveCommand, MoveCommandSaveData>();
            ////CreateMap<WalkCommand, WalkCommandSaveData>();
            ////CreateMap<AttackCommand, AttackCommandSaveData>();
            ////CreateMap<DeathCommand, DeathCommandSaveData>();
            ////CreateMap<PickUpItemCommand, PickUpItemSaveData>();
            ////CreateMap<EquipItemCommand, EquipItemSaveData>();
            ////CreateMap<UnequipItemCommand, UnequipItemSaveData>();
            ////CreateMap<DropItemCommand, DropItemSaveData>();

            ////CreateMap<MapCollection, MapCollectionSaveData>()
            ////    .ForMember(
            ////        d => d.Maps,
            ////        o => o.MapFrom(s => s)
            ////    );

        }
    }
}
