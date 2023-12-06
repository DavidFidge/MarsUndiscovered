using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components.Factories
{
    public interface ICommandCollection : ISaveable
    {
        T CreateCommand<T>(IGameWorld gameWorld) where T : BaseGameActionCommand;
        void SaveState(ISaveGameService saveGameService, IGameWorld gameWorld);
        void LoadState(ISaveGameService saveGameService, IGameWorld gameWorld);
        void AddReplayCommand(BaseGameActionCommand command);
        BaseGameActionCommand[] GetReplayCommands();
        T GetLastCommand<T>();
        BaseGameActionCommand GetCommand(uint dataCommandId);
        List<T> GetCommands<T>();
        void Initialise();
        void ReprocessReplayCommand<T>(T command) where T : BaseGameActionCommand;
    }
}