using System.Collections.Generic;
using System.Linq;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;

namespace MarsUndiscovered.Components
{
    public abstract class BaseCommandCollection<T, TState> : List<BaseGameActionCommand<TState>>, ISaveable
        where T : BaseGameActionCommand<TState>, IMementoState<TState>
    {
        public virtual void SaveState(ISaveGameStore saveGameStore)
        {
            var gameObjectSaveData = this
                .Select(go => go.GetSaveState(saveGameStore.Mapper))
                .ToList();

            saveGameStore.SaveListToStore(gameObjectSaveData);
        }

        public void LoadState(ISaveGameStore saveGameStore)
        {
            var gameObjectSaveData = saveGameStore.GetListFromStore<TState>();

            foreach (var gameObjectSaveDataItem in gameObjectSaveData)
            {
                var command = Create();

                command.SetLoadState(gameObjectSaveDataItem, saveGameStore.Mapper);

                Add(command);
            }
        }

        protected abstract T Create();
    }
}