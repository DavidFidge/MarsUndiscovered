using MarsUndiscovered.Commands;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Components
{
    public class AttackCommandCollection : BaseCommandCollection<AttackCommand, AttackCommandSaveData>
    {
        private readonly ICommandFactory _commandFactory;

        public AttackCommandCollection(ICommandFactory commandFactory, IGameWorld gameWorld) : base(gameWorld)
        {
            _commandFactory = commandFactory;
        }

        protected override AttackCommand Create()
        {
            return _commandFactory.CreateAttackCommand(GameWorld);
        }
    }
}