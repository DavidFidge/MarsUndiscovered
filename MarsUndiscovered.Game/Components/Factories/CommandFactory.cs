using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components.Factories
{
    public class CommandFactory : ICommandFactory
    {
        public ICommandFactory<MoveCommand> MoveCommandFactory { get; set; }
        public ICommandFactory<WalkCommand> WalkCommandFactory { get; set; }
        public ICommandFactory<MeleeAttackCommand> MeleeAttackCommandFactory { get; set; }
        public ICommandFactory<LineAttackCommand> LineAttackCommandFactory { get; set; }
        public ICommandFactory<LightningAttackCommand> LightningAttackCommandFactory { get; set; }
        public ICommandFactory<DeathCommand> DeathCommandFactory { get; set; }
        public ICommandFactory<PickUpItemCommand> PickUpItemCommandFactory { get; set; }
        public ICommandFactory<EquipItemCommand> EquipItemCommandFactory { get; set; }
        public ICommandFactory<UnequipItemCommand> UnequipItemCommandFactory { get; set; }
        public ICommandFactory<DropItemCommand> DropItemCommandFactory { get; set; }
        public ICommandFactory<ChangeMapCommand> ChangeMapCommandFactory { get; set; }
        public ICommandFactory<ApplyItemCommand> ApplyItemCommandFactory { get; set; }
        public ICommandFactory<ApplyShieldCommand> ApplyShieldCommandFactory { get; set; }
        public ICommandFactory<ApplyHealingBotsCommand> ApplyHealingBotsCommandFactory { get; set; }
        public ICommandFactory<WaitCommand> WaitCommandFactory { get; set; }

        public MoveCommand CreateMoveCommand(IGameWorld gameWorld)
        {
            return MoveCommandFactory.Create(gameWorld);
        }

        public WalkCommand CreateWalkCommand(IGameWorld gameWorld)
        {
            return WalkCommandFactory.Create(gameWorld);
        }
        public MeleeAttackCommand CreateMeleeAttackCommand(IGameWorld gameWorld)
        {
            return MeleeAttackCommandFactory.Create(gameWorld);
        }
        
        public LineAttackCommand CreateLineAttackCommand(IGameWorld gameWorld)
        {
            return LineAttackCommandFactory.Create(gameWorld);
        }

        public LightningAttackCommand CreateLightningAttackCommand(IGameWorld gameWorld)
        {
            return LightningAttackCommandFactory.Create(gameWorld);
        }

        public DeathCommand CreateDeathCommand(IGameWorld gameWorld)
        {
            return DeathCommandFactory.Create(gameWorld);
        }

        public PickUpItemCommand CreatePickUpItemCommand(IGameWorld gameWorld)
        {
            return PickUpItemCommandFactory.Create(gameWorld);
        }

        public EquipItemCommand CreateEquipItemCommand(IGameWorld gameWorld)
        {
            return EquipItemCommandFactory.Create(gameWorld);
        }

        public UnequipItemCommand CreateUnequipItemCommand(IGameWorld gameWorld)
        {
            return UnequipItemCommandFactory.Create(gameWorld);
        }

        public DropItemCommand CreateDropItemCommand(IGameWorld gameWorld)
        {
            return DropItemCommandFactory.Create(gameWorld);
        }

        public ChangeMapCommand CreateChangeMapCommand(IGameWorld gameWorld)
        {
            return ChangeMapCommandFactory.Create(gameWorld);
        }
        
        public ApplyItemCommand CreateApplyItemCommand(IGameWorld gameWorld)
        {
            return ApplyItemCommandFactory.Create(gameWorld);
        }
        
        public ApplyHealingBotsCommand CreateApplyHealingBotsCommand(IGameWorld gameWorld)
        {
            return ApplyHealingBotsCommandFactory.Create(gameWorld);
        }
        
        public ApplyShieldCommand CreateApplyShieldCommand(IGameWorld gameWorld)
        {
            return ApplyShieldCommandFactory.Create(gameWorld);
        }

        public WaitCommand CreateWaitCommand(IGameWorld gameWorld)
        {
            return WaitCommandFactory.Create(gameWorld);
        }
    }
}
