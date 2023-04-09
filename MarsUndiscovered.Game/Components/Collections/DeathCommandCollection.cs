using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components
{
    public class DeathCommandCollection : BaseCommandCollection<DeathCommand, DeathCommandSaveData>
    {
        private readonly ICommandFactory _commandFactory;

        public DeathCommandCollection(ICommandFactory commandFactory, IGameWorld gameWorld) : base(gameWorld)
        {
            _commandFactory = commandFactory;
        }

        protected override DeathCommand Create()
        {
            return _commandFactory.CreateDeathCommand(GameWorld);
        }
    }
}