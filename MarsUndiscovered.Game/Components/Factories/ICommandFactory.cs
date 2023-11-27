using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components.Factories
{
    public interface ICommandFactory
    {
        T CreateCommand<T>(IGameWorld gameWorld) where T : BaseGameActionCommand;
        List<BaseGameActionCommand> CreatedCommands { get; }
    }
}