using MarsUndiscovered.Commands;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Components
{
    public class ApplyShieldCommandCollection : BaseCommandCollection<ApplyShieldCommand, ApplyShieldCommandSaveData>
    {
        private readonly ICommandFactory _commandFactory;

        public ApplyShieldCommandCollection(ICommandFactory commandFactory, IGameWorld gameWorld) : base(gameWorld)
        {
            _commandFactory = commandFactory;
        }

        protected override ApplyShieldCommand Create()
        {
            return _commandFactory.CreateApplyShieldCommand(GameWorld);
        }
    }
}