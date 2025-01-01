using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components.Factories
{
    public interface ICommandCollection : ISaveable
    {
        T CreateCommand<T>(IGameWorld gameWorld) where T : BaseGameActionCommand;
        T GetLastCommand<T>();
        BaseGameActionCommand GetCommand(uint dataCommandId);
        List<T> GetCommands<T>();
        void Initialise();
    }
}