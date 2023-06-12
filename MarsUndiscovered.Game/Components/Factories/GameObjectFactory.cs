using Castle.MicroKernel;
using Castle.Windsor;

using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Services;

using GoRogue.GameFramework;

using MarsUndiscovered.Game.Components.SaveData;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components.Factories
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

        public T CreateGameObject<T>() where T : IGameObject
        {
            return ResolveWithNextId<T>();
        }

        public T CreateGameObject<T>(uint id) where T : IGameObject
        {
            return ResolveWithGivenId<T>(id);
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

        public void SaveState(ISaveGameService saveGameService, IGameWorld gameWorld)
        {
            var memento = new Memento<GameObjectFactorySaveData>(new GameObjectFactorySaveData());
            memento.State.LastId = LastId;

            saveGameService.SaveToStore(memento);
        }

        public void LoadState(ISaveGameService saveGameService, IGameWorld gameWorld)
        {
            var gameObjectFactoryData = saveGameService.GetFromStore<GameObjectFactorySaveData>();
            LastId = gameObjectFactoryData.State.LastId;
        }
    }
}
