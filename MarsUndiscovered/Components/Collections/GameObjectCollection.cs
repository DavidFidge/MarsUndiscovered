using System;
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
        public virtual void SaveState(ISaveGameStore saveGameStore)
        {
            var gameObjectSaveData = Values
                .Select(go => go.GetSaveState(saveGameStore.Mapper))
                .ToList();

            saveGameStore.SaveListToStore(gameObjectSaveData);
        }

        public void LoadState(ISaveGameStore saveGameStore)
        {
            var gameObjectSaveData = saveGameStore.GetListFromStore<TState>();

            foreach (var gameObjectSaveDataItem in gameObjectSaveData)
            {
                var gameObject = Create(gameObjectSaveDataItem.State.Id);

                gameObject.SetLoadState(gameObjectSaveDataItem, saveGameStore.Mapper);

                Add(gameObject.ID, gameObject);
            }
        }

        protected abstract T Create(uint id);
    }
}