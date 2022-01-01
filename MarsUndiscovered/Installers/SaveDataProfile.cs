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
        private readonly ICommandFactory _commandFactory;

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

            CreateMap<GameWorldSaveData, GameWorld>();

            CreateMap<MoveCommandSaveData, MoveCommand>();
            CreateMap<WalkCommandSaveData, WalkCommand>();
            CreateMap<AttackCommandSaveData, AttackCommand>();
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
            CreateMap<GameWorld, GameWorldSaveData>();
            CreateMap<GameObjectFactory, GameObjectFactoryData>();

            CreateMap<MoveCommand, MoveCommandSaveData>();
            CreateMap<WalkCommand, WalkCommandSaveData>();
            CreateMap<AttackCommand, AttackCommandSaveData>();
        }
    }
}
