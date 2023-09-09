using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components
{
    public class EnchantItemCommandCollection : BaseCommandCollection<EnchantItemCommand, EnchantItemCommandSaveData>
    {
        private readonly ICommandFactory _commandFactory;

        public EnchantItemCommandCollection(ICommandFactory commandFactory, IGameWorld gameWorld) : base(gameWorld)
        {
            _commandFactory = commandFactory;
        }

        protected override EnchantItemCommand Create()
        {
            return _commandFactory.CreateEnchantItemCommand(GameWorld);
        }
    }
}