using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components
{
    public class ApplyMachineCommandCollection : BaseCommandCollection<ApplyMachineCommand, ApplyMachineCommandSaveData>
    {
        private readonly ICommandFactory _commandFactory;

        public ApplyMachineCommandCollection(ICommandFactory commandFactory, IGameWorld gameWorld) : base(gameWorld)
        {
            _commandFactory = commandFactory;
        }

        protected override ApplyMachineCommand Create()
        {
            return _commandFactory.CreateApplyMachineCommand(GameWorld);
        }
    }
}