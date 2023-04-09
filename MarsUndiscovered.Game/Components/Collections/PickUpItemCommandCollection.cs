using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components
{
    public class PickUpItemCommandCollection : BaseCommandCollection<PickUpItemCommand, PickUpItemSaveData>
    {
        private readonly ICommandFactory _commandFactory;

        public PickUpItemCommandCollection(ICommandFactory commandFactory, IGameWorld gameWorld) : base(gameWorld)
        {
            _commandFactory = commandFactory;
        }

        protected override PickUpItemCommand Create()
        {
            return _commandFactory.CreatePickUpItemCommand(GameWorld);
        }
    }
}