using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components
{
    public class LightningAttackCommandCollection : BaseCommandCollection<LightningAttackCommand, LightningAttackCommandSaveData>
    {
        private readonly ICommandFactory _commandFactory;

        public LightningAttackCommandCollection(ICommandFactory commandFactory, IGameWorld gameWorld) : base(gameWorld)
        {
            _commandFactory = commandFactory;
        }

        protected override LightningAttackCommand Create()
        {
            return _commandFactory.CreateLightningAttackCommand(GameWorld);
        }
    }
}