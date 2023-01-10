using Castle.MicroKernel;
using Castle.Windsor;

using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Services;

using GoRogue.GameFramework;

using MarsUndiscovered.Components.SaveData;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Components.Factories
{
    public class GameObjectFactory : IGameObjectFactory
    {
        private readonly IWindsorContainer _container;
        private IGameWorld _gameWorld;

        public void Initialise(IGameWorld gameWorld)
        {
            LastId = 0;
            GameObjects.Clear();
            _gameWorld = gameWorld;
        }

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

        public Ship CreateShip()
        {
            return ResolveWithNextId<Ship>();
        }

        public Ship CreateShip(uint id)
        {
            return ResolveWithGivenId<Ship>(id);
        }

        private T ResolveWithNextId<T>() where T : IGameObject
        {
            var gameObject = _container.Resolve<T>(new Arguments { { "gameWorld", _gameWorld }, { "id", GetNextId() } });
            GameObjects.Add(gameObject.ID, gameObject);

            return gameObject;
        }

        private T ResolveWithGivenId<T>(uint id) where T : IGameObject
        {
            var gameObject = _container.Resolve<T>(new Arguments { { "gameWorld", _gameWorld }, { "id", id } });
            GameObjects.Add(gameObject.ID, gameObject);

            return gameObject;
        }

        public void SaveState(ISaveGameService saveGameService)
        {
            var memento = new Memento<GameObjectFactorySaveData>(new GameObjectFactorySaveData());
            memento.State.LastId = LastId;

            saveGameService.SaveToStore(memento);
        }

        public void LoadState(ISaveGameService saveGameService)
        {
            var gameObjectFactoryData = saveGameService.GetFromStore<GameObjectFactorySaveData>();
            LastId = gameObjectFactoryData.State.LastId;
        }
    }
}
