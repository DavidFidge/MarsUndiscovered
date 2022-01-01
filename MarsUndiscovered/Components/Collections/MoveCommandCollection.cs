using MarsUndiscovered.Commands;
using MarsUndiscovered.Components.Factories;

namespace MarsUndiscovered.Components
{
    public class MoveCommandCollection : BaseCommandCollection<MoveCommand, MoveCommandSaveData>
    {
        private readonly ICommandFactory _commandFactory;

        public MoveCommandCollection(ICommandFactory commandFactory)
        {
            _commandFactory = commandFactory;
        }

        protected override MoveCommand Create()
        {
            return _commandFactory.CreateMoveCommand();
        }
    }
}