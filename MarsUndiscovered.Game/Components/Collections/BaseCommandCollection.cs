using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components
{
    public abstract class BaseCommandCollection<T, TState> : List<BaseMarsGameActionCommand<TState>>, ISaveable
        where T : BaseMarsGameActionCommand<TState>, IMementoState<TState>
    {
        protected readonly IGameWorld GameWorld;

        public BaseCommandCollection(IGameWorld gameWorld)
        {
            GameWorld = gameWorld;
        }

        public virtual void SaveState(ISaveGameService saveGameService, IGameWorld gameWorld)
        {
            var gameObjectSaveData = this
                .Select(go => go.GetSaveState())
                .ToList();

            saveGameService.SaveListToStore(gameObjectSaveData);
        }

        public void LoadState(ISaveGameService saveGameService, IGameWorld gameWorld)
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
