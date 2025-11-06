using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.SaveData;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components
{
    public class GameObjectCollection<T, TState> : Dictionary<uint, T>, ISaveable
        where T : IMarsGameObject, IMementoState<TState>
        where TState : GameObjectSaveData
    {
        protected readonly IGameObjectFactory _gameObjectFactory;

        public GameObjectCollection(IGameObjectFactory gameObjectFactory)
        {
            _gameObjectFactory = gameObjectFactory;
        }

        public virtual void SaveState(ISaveGameService saveGameService, IGameWorld gameWorld)
        {
            var gameObjectSaveData = Values
                .Select(go => go.GetSaveState())
                .ToList();

            saveGameService.SaveListToStore(gameObjectSaveData);
        }

        public void LoadState(ISaveGameService saveGameService, IGameWorld gameWorld)
        {
            var saveData = saveGameService.GetListFromStore<TState>();

            foreach (var gameObjectSaveDataItem in saveData)
            {
                var gameObject = Create(gameObjectSaveDataItem.State.Id);

                gameObject.SetLoadState(gameObjectSaveDataItem);

                Add(gameObject.ID, gameObject);
            }

            AfterCollectionLoaded(gameWorld, saveData);
        }

        protected virtual void AfterCollectionLoaded(IGameWorld gameWorld, IList<IMemento<TState>> saveData)
        {
        }

        protected virtual T Create(uint id)
        {
            return _gameObjectFactory.CreateGameObject<T>(id);
        }
    }
}
