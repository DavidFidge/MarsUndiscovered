using System.Linq;

using AutoMapper;

using GoRogue.GameFramework;

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
            CreateMap<GameObjectSaveData, IGameObject>()
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
                .ConstructUsing(sd => _gameObjectFactory.CreateMonster(sd.Id));

            CreateMap<GameWorldSaveData, GameWorld>();
            CreateMap<GameObjectFactoryData, GameObjectFactory>();
        }

        private void MapForSave()
        {
            CreateMap<IGameObject, GameObjectSaveData>()
                .Include<Wall, WallSaveData>()
                .Include<Floor, FloorSaveData>()
                .Include<Player, PlayerSaveData>();

            CreateMap<Wall, WallSaveData>();
            CreateMap<Floor, FloorSaveData>();
            CreateMap<Player, PlayerSaveData>();
            CreateMap<Monster, MonsterSaveData>();
            CreateMap<GameWorld, GameWorldSaveData>();
            CreateMap<GameObjectFactory, GameObjectFactoryData>();
        }
    }
}
