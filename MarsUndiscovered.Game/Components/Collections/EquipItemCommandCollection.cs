using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components
{
    public class EquipItemCommandCollection : BaseCommandCollection<EquipItemCommand, EquipItemSaveData>
    {
        private readonly ICommandFactory _commandFactory;

        public EquipItemCommandCollection(ICommandFactory commandFactory, IGameWorld gameWorld) : base(gameWorld)
        {
            _commandFactory = commandFactory;
        }

        protected override EquipItemCommand Create()
        {
            return _commandFactory.CreateEquipItemCommand(GameWorld);
        }
    }
}