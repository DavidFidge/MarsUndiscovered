using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Commands
{
    public interface IBaseMarsGameActionCommand
    {
        IGameWorld GameWorld { get; }
        ICommandFactory CommandFactory { get; set; }
        bool InterruptsMovement { get; }
    }
}