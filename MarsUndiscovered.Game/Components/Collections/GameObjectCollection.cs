using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;

using MarsUndiscovered.Game.Components.SaveData;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components
{
    public abstract class GameObjectCollection<T, TState> : Dictionary<uint, T>, ISaveable
        where T : IMarsGameObject, IMementoState<TState>
        where TState : GameObjectSaveData
    {
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

            AfterCollectionLoaded(saveData);
        }

        protected virtual void AfterCollectionLoaded(IList<IMemento<TState>> saveData)
        {
        }

        protected abstract T Create(uint id);
    }
}