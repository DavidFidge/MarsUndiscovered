using MarsUndiscovered.Commands;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Components
{
    public class UnequipItemCommandCollection : BaseCommandCollection<UnequipItemCommand, UnequipItemSaveData>
    {
        private readonly ICommandFactory _commandFactory;

        public UnequipItemCommandCollection(ICommandFactory commandFactory, IGameWorld gameWorld) : base(gameWorld)
        {
            _commandFactory = commandFactory;
        }

        protected override UnequipItemCommand Create()
        {
            return _commandFactory.CreateUnequipItemCommand(GameWorld);
        }
    }
}