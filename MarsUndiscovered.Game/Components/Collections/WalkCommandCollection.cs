using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components
{
    public class WalkCommandCollection : BaseCommandCollection<WalkCommand, WalkCommandSaveData>
    {
        private readonly ICommandFactory _commandFactory;

        public WalkCommandCollection(ICommandFactory commandFactory, IGameWorld gameWorld) : base(gameWorld)
        {
            _commandFactory = commandFactory;
        }

        protected override WalkCommand Create()
        {
            return _commandFactory.CreateWalkCommand(GameWorld);
        }
    }
}