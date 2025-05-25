using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components.Factories
{
    public interface ICommandCollection : ISaveable
    {
        T CreateCommand<T>(IGameWorld gameWorld) where T : BaseGameActionCommand;
        T GetLastCommand<T>() where T : BaseGameActionCommand;
        BaseGameActionCommand GetCommand(uint dataCommandId);
        List<T> GetCommands<T>() where T : BaseGameActionCommand;
        void Initialise();
    }
}