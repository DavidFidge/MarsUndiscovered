using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Commands
{
    public class EnchantItemCommand : BaseMarsGameActionCommand<EnchantItemCommandSaveData>
    {
        public Item Target { get; private set; }

        private int _oldEnchantLevel;
        private int _newEnchantLevel;
        
        public EnchantItemCommand(IGameWorld gameWorld) : base(gameWorld)
        {
            EndsPlayerTurn = true;
            PersistForReplay = true;
        }

        public void Initialise(Item target)
        {
            Target = target;
        }

        public override IMemento<EnchantItemCommandSaveData> GetSaveState()
        {
            var memento = new Memento<EnchantItemCommandSaveData>(new EnchantItemCommandSaveData());
            base.PopulateSaveState(memento.State);

            memento.State.TargetId = Target.ID;
            memento.State.OldEnchantLevel = _oldEnchantLevel;
            memento.State.NewEnchantLevel = _newEnchantLevel;

            return memento;
        }

        public override void SetLoadState(IMemento<EnchantItemCommandSaveData> memento)
        {
            base.PopulateLoadState(memento.State);

            Target = (Item)GameWorld.GameObjects[memento.State.TargetId];
            _oldEnchantLevel = memento.State.OldEnchantLevel;
            _newEnchantLevel = memento.State.NewEnchantLevel;
        }

        protected override CommandResult ExecuteInternal()
        {
            var canEnchant = GameWorld.Inventory.CanTypeBeEnchanted(Target);

            if (!canEnchant)
                return Result(CommandResult.NoMove(this, $"{GameWorld.Inventory.ItemTypeDiscoveries.GetInventoryDescriptionAsSingleItem(Target)} cannot be enchanted."));
            
            _oldEnchantLevel = Target.EnchantmentLevel;
            _newEnchantLevel = _oldEnchantLevel + 1;
            
            Target.Enchant();
            
            var message =
                $"The {GameWorld.Inventory.ItemTypeDiscoveries.GetInventoryDescriptionWithoutPrefix(Target)} glows with a blue light. It's enchantment level has increased.";
            
            var commandResult = CommandResult.Success(this, message);

            return Result(commandResult);
        }

        protected override void UndoInternal()
        {
        }
    }
}
