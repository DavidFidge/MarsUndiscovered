using MarsUndiscovered.Commands;
using MarsUndiscovered.Components.Factories;

namespace MarsUndiscovered.Components
{
    public class AttackCommandCollection : BaseCommandCollection<AttackCommand, AttackCommandSaveData>
    {
        private readonly ICommandFactory _commandFactory;

        public AttackCommandCollection(ICommandFactory commandFactory)
        {
            _commandFactory = commandFactory;
        }

        protected override AttackCommand Create()
        {
            return _commandFactory.CreateAttackCommand();
        }
    }
}