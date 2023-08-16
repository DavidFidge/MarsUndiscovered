using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components.Factories
{
    public interface ICommandFactory
    {
        MoveCommand CreateMoveCommand(IGameWorld gameWorld);
        WalkCommand CreateWalkCommand(IGameWorld gameWorld);
        MeleeAttackCommand CreateMeleeAttackCommand(IGameWorld gameWorld);
        LineAttackCommand CreateLineAttackCommand(IGameWorld gameWorld);
        LightningAttackCommand CreateLightningAttackCommand(IGameWorld gameWorld);
        DeathCommand CreateDeathCommand(IGameWorld gameWorld);
        PickUpItemCommand CreatePickUpItemCommand(IGameWorld gameWorld);
        EquipItemCommand CreateEquipItemCommand(IGameWorld gameWorld);
        UnequipItemCommand CreateUnequipItemCommand(IGameWorld gameWorld);
        DropItemCommand CreateDropItemCommand(IGameWorld gameWorld);
        ChangeMapCommand CreateChangeMapCommand(IGameWorld gameWorld);
        ApplyItemCommand CreateApplyItemCommand(IGameWorld gameWorld);
        ApplyHealingBotsCommand CreateApplyHealingBotsCommand(IGameWorld gameWorld);
        ApplyShieldCommand CreateApplyShieldCommand(IGameWorld gameWorld);
        WaitCommand CreateWaitCommand(IGameWorld gameWorld);
    }
}