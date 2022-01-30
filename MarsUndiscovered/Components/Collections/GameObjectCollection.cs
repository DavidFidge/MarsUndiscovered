using System.Collections.Generic;
using System.Linq;

using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;

using MarsUndiscovered.Components.SaveData;

namespace MarsUndiscovered.Components
{
    public abstract class GameObjectCollection<T, TState> : Dictionary<uint, T>, ISaveable
        where T : IMarsGameObject, IMementoState<TState>
        where TState : GameObjectSaveData
    {
        public virtual void SaveState(ISaveGameService saveGameService)
        {
            var gameObjectSaveData = Values
                .Select(go => go.GetSaveState())
                .ToList();

            saveGameService.SaveListToStore(gameObjectSaveData);
        }

        public void LoadState(ISaveGameService saveGameService)
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