using MarsUndiscovered.Commands;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Components.Factories
{
    public interface ICommandFactory
    {
        MoveCommand CreateMoveCommand(IGameWorld gameWorld);
        WalkCommand CreateWalkCommand(IGameWorld gameWorld);
        AttackCommand CreateAttackCommand(IGameWorld gameWorld);
        LightningAttackCommand CreateLightningAttackCommand(IGameWorld gameWorld);
        DeathCommand CreateDeathCommand(IGameWorld gameWorld);
        PickUpItemCommand CreatePickUpItemCommand(IGameWorld gameWorld);
        EquipItemCommand CreateEquipItemCommand(IGameWorld gameWorld);
        UnequipItemCommand CreateUnequipItemCommand(IGameWorld gameWorld);
        DropItemCommand CreateDropItemCommand(IGameWorld gameWorld);
        ChangeMapCommand CreateChangeMapCommand(IGameWorld gameWorld);
    }
}