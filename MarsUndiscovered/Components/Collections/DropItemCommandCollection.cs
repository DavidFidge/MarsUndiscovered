using MarsUndiscovered.Commands;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Components
{
    public class DropItemCommandCollection : BaseCommandCollection<DropItemCommand, DropItemSaveData>
    {
        private readonly ICommandFactory _commandFactory;

        public DropItemCommandCollection(ICommandFactory commandFactory, IGameWorld gameWorld) : base(gameWorld)
        {
            _commandFactory = commandFactory;
        }

        protected override DropItemCommand Create()
        {
            return _commandFactory.CreateDropItemCommand(GameWorld);
        }
    }
}