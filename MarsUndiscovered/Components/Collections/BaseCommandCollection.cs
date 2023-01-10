using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Components
{
    public abstract class BaseCommandCollection<T, TState> : List<BaseStatefulGameActionCommand<TState>>, ISaveable
        where T : BaseStatefulGameActionCommand<TState>, IMementoState<TState>
    {
        protected readonly IGameWorld GameWorld;

        public BaseCommandCollection(IGameWorld gameWorld)
        {
            GameWorld = gameWorld;
        }

        public virtual void SaveState(ISaveGameService saveGameService)
        {
            var gameObjectSaveData = this
                .Select(go => go.GetSaveState())
                .ToList();

            saveGameService.SaveListToStore(gameObjectSaveData);
        }

        public void LoadState(ISaveGameService saveGameService)
        {
            var gameObjectSaveData = saveGameService.GetListFromStore<TState>();

            foreach (var gameObjectSaveDataItem in gameObjectSaveData)
            {
                var command = Create();

                command.SetLoadState(gameObjectSaveDataItem);

                Add(command);
            }
        }

        protected abstract T Create();
    }
}
