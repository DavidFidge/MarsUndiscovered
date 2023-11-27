using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components
{
    public class UndoCommandCollection : BaseCommandCollection<UndoCommand, UndoCommandSaveData>
    {
        private readonly ICommandFactory _commandFactory;

        public UndoCommandCollection(ICommandFactory commandFactory, IGameWorld gameWorld) : base(gameWorld)
        {
            _commandFactory = commandFactory;
        }

        protected override UndoCommand Create()
        {
            return _commandFactory.CreateUndoCommand(GameWorld);
        }
    }
}