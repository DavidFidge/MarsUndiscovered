using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Services;
using GoRogue.GameFramework;
using MarsUndiscovered.Game.DependencyInjection;
using MarsUndiscovered.Game.Components.SaveData;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components.Factories
{
    public class GameObjectFactory : IGameObjectFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private IGameWorld _gameWorld;

        public void Initialise(IGameWorld gameWorld)
        {
            LastId = 0;
            GameObjects.Clear();
            _gameWorld = gameWorld;
        }

        public uint LastId { get; private set; }
        public IDictionary<uint, IGameObject> GameObjects { get; } = new Dictionary<uint, IGameObject>();

        private uint GetNextId()
        {
            return ++LastId;
        }

        public GameObjectFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
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
            var gameObject = _serviceProvider.CreateWithInjectedProperties<T>(_gameWorld, GetNextId());
            GameObjects.Add(gameObject.ID, gameObject);

            return gameObject;
        }

        private T ResolveWithGivenId<T>(uint id) where T : IGameObject
        {
            var gameObject = _serviceProvider.CreateWithInjectedProperties<T>(_gameWorld, id);
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
