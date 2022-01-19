using System;

using MarsUndiscovered.Commands;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Components.Factories
{
    public class CommandFactory : ICommandFactory
    {
        public ICommandFactory<MoveCommand> MoveCommandFactory { get; set; }
        public ICommandFactory<WalkCommand> WalkCommandFactory { get; set; }
        public ICommandFactory<AttackCommand> AttackCommandFactory { get; set; }
        public ICommandFactory<DeathCommand> DeathCommandFactory { get; set; }
        public ICommandFactory<PickUpItemCommand> PickUpItemCommandFactory { get; set; }
        public ICommandFactory<EquipItemCommand> EquipItemCommandFactory { get; set; }
        public ICommandFactory<UnequipItemCommand> UnequipItemCommandFactory { get; set; }
        public ICommandFactory<DropItemCommand> DropItemCommandFactory { get; set; }
        public ICommandFactory<ChangeMapCommand> ChangeMapCommandFactory { get; set; }

        public MoveCommand CreateMoveCommand(IGameWorld gameWorld)
        {
            return MoveCommandFactory.Create(gameWorld);
        }

        public WalkCommand CreateWalkCommand(IGameWorld gameWorld)
        {
            return WalkCommandFactory.Create(gameWorld);
        }
        public AttackCommand CreateAttackCommand(IGameWorld gameWorld)
        {
            return AttackCommandFactory.Create(gameWorld);
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
    }
}
