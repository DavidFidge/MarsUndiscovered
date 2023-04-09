using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components
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