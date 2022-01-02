using MarsUndiscovered.Commands;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Components
{
    public class MoveCommandCollection : BaseCommandCollection<MoveCommand, MoveCommandSaveData>
    {
        private readonly ICommandFactory _commandFactory;

        public MoveCommandCollection(ICommandFactory commandFactory, IGameWorld gameWorld) : base(gameWorld)
        {
            _commandFactory = commandFactory;
        }

        protected override MoveCommand Create()
        {
            return _commandFactory.CreateMoveCommand(GameWorld);
        }
    }
}