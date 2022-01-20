using System.Collections.Generic;

using Castle.MicroKernel;
using Castle.Windsor;

using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Services;

using GoRogue.GameFramework;

using MarsUndiscovered.Components.SaveData;

namespace MarsUndiscovered.Components.Factories
{
    public class GameObjectFactory : IGameObjectFactory
    {
        private readonly IWindsorContainer _container;

        public uint LastId { get; private set; } = 0;
        public IDictionary<uint, IGameObject> GameObjects { get; } = new Dictionary<uint, IGameObject>();

        private uint GetNextId()
        {
            return ++LastId;
        }

        public GameObjectFactory(IWindsorContainer container)
        {
            _container = container;
        }

        public void Reset()
        {
            LastId = 0;
            GameObjects.Clear();
        }

        public Player CreatePlayer()
        {
            return ResolveWithNextId<Player>();
        }

        public Player CreatePlayer(uint id)
        {
            return ResolveWithGivenId<Player>(id);
        }

        public Wall CreateWall()
        {
            return ResolveWithNextId<Wall>();
        }

        public Wall CreateWall(uint id)
        {
            return ResolveWithGivenId<Wall>(id);
        }

        public Floor CreateFloor()
        {
            return ResolveWithNextId<Floor>();
        }

        public Floor CreateFloor(uint id)
        {
            return ResolveWithGivenId<Floor>(id);
        }

        public Monster CreateMonster()
        {
            return ResolveWithNextId<Monster>();
        }

        public Monster CreateMonster(uint id)
        {
            return ResolveWithGivenId<Monster>(id);
        }

        public Item CreateItem()
        {
            return ResolveWithNextId<Item>();
        }

        public Item CreateItem(uint id)
        {
            return ResolveWithGivenId<Item>(id);
        }

        public MapExit CreateMapExit()
        {
            return ResolveWithNextId<MapExit>();
        }

        public MapExit CreateMapExit(uint id)
        {
            return ResolveWithGivenId<MapExit>(id);
        }

        private T ResolveWithNextId<T>() where T : IGameObject
        {
            var gameObject = _container.Resolve<T>(new Arguments { { "id", GetNextId() } });
            GameObjects.Add(gameObject.ID, gameObject);

            return gameObject;
        }

        private T ResolveWithGivenId<T>(uint id) where T : IGameObject
        {
            var gameObject = _container.Resolve<T>(new Arguments { { "id", id } });
            GameObjects.Add(gameObject.ID, gameObject);

            return gameObject;
        }

        public void SaveState(ISaveGameService saveGameService)
        {
            var gameObjectFactoryData = Memento<GameObjectFactorySaveData>.CreateWithAutoMapper(this, saveGameService.Mapper);
            saveGameService.SaveToStore(gameObjectFactoryData);
        }

        public void LoadState(ISaveGameService saveGameService)
        {
            var gameObjectFactoryData = saveGameService.GetFromStore<GameObjectFactorySaveData>();
            LastId = gameObjectFactoryData.State.LastId;
        }
    }
}
