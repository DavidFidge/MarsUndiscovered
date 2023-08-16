using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components
{
    public class WaitCommandCollection : BaseCommandCollection<WaitCommand, WaitCommandSaveData>
    {
        private readonly ICommandFactory _commandFactory;

        public WaitCommandCollection(ICommandFactory commandFactory, IGameWorld gameWorld) : base(gameWorld)
        {
            _commandFactory = commandFactory;
        }

        protected override WaitCommand Create()
        {
            return _commandFactory.CreateWaitCommand(GameWorld);
        }
    }
}