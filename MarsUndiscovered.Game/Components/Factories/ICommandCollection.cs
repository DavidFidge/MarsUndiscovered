using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components.Factories
{
    public interface ICommandCollection
    {
        T CreateCommand<T>(IGameWorld gameWorld) where T : BaseGameActionCommand;
        T GetLastCommand<T>() where T : BaseGameActionCommand;
        List<T> GetLastCommands<T>() where T : BaseGameActionCommand;
        void Initialise();
        void ClearCommandsOnNextAdd();
        BaseGameActionCommand GetCommand(uint commandId);
    }
}