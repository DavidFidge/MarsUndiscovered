using MarsUndiscovered.Commands;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Components
{
    public class AttackCommandCollection : BaseCommandCollection<MeleeAttackCommand, MeleeAttackCommandSaveData>
    {
        private readonly ICommandFactory _commandFactory;

        public AttackCommandCollection(ICommandFactory commandFactory, IGameWorld gameWorld) : base(gameWorld)
        {
            _commandFactory = commandFactory;
        }

        protected override MeleeAttackCommand Create()
        {
            return _commandFactory.CreateMeleeAttackCommand(GameWorld);
        }
    }
}