using FrigidRogue.MonoGame.Core.Components;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Commands
{
    public class EnchantItemCommand : BaseMarsGameActionCommand
    {
        public Item Target { get; set; }

        public EnchantItemCommand(IGameWorld gameWorld) : base(gameWorld)
        {
            EndsPlayerTurn = true;
        }

        public void Initialise(Item target)
        {
            Target = target;
        }

        protected override CommandResult ExecuteInternal()
        {
            var canEnchant = GameWorld.Inventory.CanTypeBeEnchanted(Target);

            if (!canEnchant)
                return Result(CommandResult.NoMove(this, $"{GameWorld.Inventory.ItemTypeDiscoveries.GetInventoryDescriptionAsSingleItem(Target)} cannot be enchanted."));

            Target.Enchant();

            var message =
                $"The {GameWorld.Inventory.ItemTypeDiscoveries.GetInventoryDescriptionWithoutPrefix(Target)} glows with a blue light. It's enchantment level has increased.";
            
            var commandResult = CommandResult.Success(this, message);

            return Result(commandResult);
        }
    }
}
