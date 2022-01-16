using MarsUndiscovered.Commands;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Components
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