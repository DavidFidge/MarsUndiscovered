using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Commands
{
    public interface IBaseMarsGameActionCommand
    {
        IGameWorld GameWorld { get; }
        ICommandFactory CommandFactory { get; set; }
        bool InterruptsMovement { get; }
    }
}