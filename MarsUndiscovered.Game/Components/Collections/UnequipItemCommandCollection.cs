using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components
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