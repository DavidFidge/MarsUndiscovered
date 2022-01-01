using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Commands;

namespace MarsUndiscovered.Components.Factories
{
    public class CommandFactory : ICommandFactory
    {
        private IFactory<MoveCommand> MoveCommandFactory { get; set; }
        private IFactory<WalkCommand> WalkCommandFactory { get; set; }
        private IFactory<AttackCommand> AttackCommandFactory { get; set; }

        public MoveCommand CreateMoveCommand()
        {
            return MoveCommandFactory.Create();
        }

        public WalkCommand CreateWalkCommand()
        {
            return WalkCommandFactory.Create();
        }
        public AttackCommand CreateAttackCommand()
        {
            return AttackCommandFactory.Create();
        }
    }
}
