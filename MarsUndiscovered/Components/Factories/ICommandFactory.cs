using System.Collections.Generic;
using MarsUndiscovered.Commands;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Components.Factories
{
    public interface ICommandFactory
    {
        MoveCommand CreateMoveCommand(IGameWorld gameWorld);
        WalkCommand CreateWalkCommand(IGameWorld gameWorld);
        AttackCommand CreateAttackCommand(IGameWorld gameWorld);
    }
}