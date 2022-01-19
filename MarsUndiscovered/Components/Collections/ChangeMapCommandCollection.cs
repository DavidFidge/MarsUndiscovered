using MarsUndiscovered.Commands;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Components
{
    public class ChangeMapCommandCollection : BaseCommandCollection<ChangeMapCommand, ChangeMapSaveData>
    {
        private readonly ICommandFactory _commandFactory;

        public ChangeMapCommandCollection(ICommandFactory commandFactory, IGameWorld gameWorld) : base(gameWorld)
        {
            _commandFactory = commandFactory;
        }

        protected override ChangeMapCommand Create()
        {
            return _commandFactory.CreateChangeMapCommand(GameWorld);
        }
    }
}