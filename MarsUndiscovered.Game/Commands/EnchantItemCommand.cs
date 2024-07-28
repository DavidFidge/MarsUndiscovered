using FrigidRogue.MonoGame.Core.Components;

using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Commands
{
    public class EnchantItemCommand : BaseMarsGameActionCommand<EnchantItemCommandSaveData>
    {
        public Item Target => GameWorld.Items[_data.TargetId];

        private int _oldEnchantLevel => _data.OldEnchantLevel;
        private int _newEnchantLevel => _data.NewEnchantLevel;
        
        public EnchantItemCommand(IGameWorld gameWorld) : base(gameWorld)
        {
            EndsPlayerTurn = true;
        }

        public void Initialise(Item target)
        {
            _data.TargetId = target.ID;
        }

        protected override CommandResult ExecuteInternal()
        {
            var canEnchant = GameWorld.Inventory.CanTypeBeEnchanted(Target);

            if (!canEnchant)
                return Result(CommandResult.NoMove(this, $"{GameWorld.Inventory.ItemTypeDiscoveries.GetInventoryDescriptionAsSingleItem(Target)} cannot be enchanted."));
            
            _data.OldEnchantLevel = Target.EnchantmentLevel;
            _data.NewEnchantLevel = _oldEnchantLevel + 1;
            
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
