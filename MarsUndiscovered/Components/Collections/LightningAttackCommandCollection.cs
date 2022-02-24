using MarsUndiscovered.Commands;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Components
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