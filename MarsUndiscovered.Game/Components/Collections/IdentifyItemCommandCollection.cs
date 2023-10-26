using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components
{
    public class IdentifyItemCommandCollection : BaseCommandCollection<IdentifyItemCommand, IdentifyItemCommandSaveData>
    {
        private readonly ICommandFactory _commandFactory;

        public IdentifyItemCommandCollection(ICommandFactory commandFactory, IGameWorld gameWorld) : base(gameWorld)
        {
            _commandFactory = commandFactory;
        }

        protected override IdentifyItemCommand Create()
        {
            return _commandFactory.CreateIdentifyItemCommand(GameWorld);
        }
    }
}