using MarsUndiscovered.Commands;
using MarsUndiscovered.Components.Factories;

namespace MarsUndiscovered.Components
{
    public class WalkCommandCollection : BaseCommandCollection<WalkCommand, WalkCommandSaveData>
    {
        private readonly ICommandFactory _commandFactory;

        public WalkCommandCollection(ICommandFactory commandFactory)
        {
            _commandFactory = commandFactory;
        }

        protected override WalkCommand Create()
        {
            return _commandFactory.CreateWalkCommand();
        }
    }
}