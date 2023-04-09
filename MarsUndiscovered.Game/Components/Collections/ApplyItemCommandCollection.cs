using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components
{
    public class ApplyItemCommandCollection : BaseCommandCollection<ApplyItemCommand, ApplyItemCommandSaveData>
    {
        private readonly ICommandFactory _commandFactory;

        public ApplyItemCommandCollection(ICommandFactory commandFactory, IGameWorld gameWorld) : base(gameWorld)
        {
            _commandFactory = commandFactory;
        }

        protected override ApplyItemCommand Create()
        {
            return _commandFactory.CreateApplyItemCommand(GameWorld);
        }
    }
}