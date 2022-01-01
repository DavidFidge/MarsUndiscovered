using System.Collections.Generic;
using MarsUndiscovered.Commands;

namespace MarsUndiscovered.Components.Factories
{
    public interface ICommandFactory
    {
        MoveCommand CreateMoveCommand();
        WalkCommand CreateWalkCommand();
        AttackCommand CreateAttackCommand();
    }
}