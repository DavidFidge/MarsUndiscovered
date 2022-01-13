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
            CreateMap<GameObjectSaveData, IMarsGameObject>()
                .Include<WallSaveData, Wall>()
                .Include<FloorSaveData, Floor>()
                .Include<PlayerSaveData, Player>();

            CreateMap<WallSaveData, Wall>()
                .ConstructUsing(sd => _gameObjectFactory.CreateWall(sd.Id));

            CreateMap<FloorSaveData, Floor>()
                .ConstructUsing(sd => _gameObjectFactory.CreateFloor(sd.Id));

            CreateMap<PlayerSaveData, Player>()
                .ConstructUsing(sd => _gameObjectFactory.CreatePlayer(sd.Id));

            CreateMap<MonsterSaveData, Monster>()
                .ConstructUsing(sd => _gameObjectFactory.CreateMonster(sd.Id))
                .ForMember(d => d.Breed, o => o.MapFrom(s => Breed.GetBreed(s.BreedName)));

            CreateMap<ItemSaveData, Item>()
                .ConstructUsing(sd => _gameObjectFactory.CreateItem(sd.Id))
                .ForMember(d => d.ItemType, o => o.MapFrom(s => ItemType.GetItemType(s.ItemTypeName)));

            CreateMap<InventorySaveData, Inventory>()
                .ForMember(d => d.Items, o => o.Ignore())
                .ForMember(d => d.ItemKeyAssignments, o => o.Ignore())
                .ForMember(d => d.CallItem, o => o.Ignore())
                .ForMember(d => d.CallItemType, o => o.Ignore());

            CreateMap<GameWorldSaveData, GameWorld>();
            CreateMap<MoveCommandSaveData, MoveCommand>();
            CreateMap<WalkCommandSaveData, WalkCommand>();
            CreateMap<AttackCommandSaveData, AttackCommand>();
            CreateMap<DeathCommandSaveData, DeathCommand>();
        }

        private void MapForSave()
        {
            CreateMap<IMarsGameObject, GameObjectSaveData>()
                .Include<Wall, WallSaveData>()
                .Include<Floor, FloorSaveData>()
                .Include<Player, PlayerSaveData>();

            CreateMap<Wall, WallSaveData>();
            CreateMap<Floor, FloorSaveData>();
            CreateMap<Player, PlayerSaveData>();
            CreateMap<Monster, MonsterSaveData>();
            CreateMap<Item, ItemSaveData>();
            CreateMap<GameWorld, GameWorldSaveData>();
            CreateMap<GameObjectFactory, GameObjectFactoryData>();

            CreateMap<Inventory, InventorySaveData>()
                .ForMember(d => d.ItemIds, o => o.MapFrom(s => s.Items.Select(i => i.ID).ToList()))
                .ForMember(d => d.ItemKeyAssignments, o => o.MapFrom(s => s.ItemKeyAssignments.ToDictionary(k => k.Key, v => v.Value.Select(i => i.ID).ToList())))
                .ForMember(d => d.CallItem, o => o.MapFrom(s => s.CallItem.ToDictionary(k => k.Key.ID, v => v.Value)))
                .ForMember(d => d.CallItemType, o => o.MapFrom(s => s.CallItemType.ToDictionary(k => k.Key.Name, v => v.Value)));

            CreateMap<MoveCommand, MoveCommandSaveData>();
            CreateMap<WalkCommand, WalkCommandSaveData>();
            CreateMap<AttackCommand, AttackCommandSaveData>();
            CreateMap<DeathCommand, DeathCommandSaveData>();
        }
    }
}
