using FrigidRogue.MonoGame.Core.Components;
using GoRogue.GameFramework;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Commands
{
    public class DropItemCommand : BaseMarsGameActionCommand
    {
        public Item Item { get; set; }
        public IGameObject GameObject { get; set; }

        public DropItemCommand(IGameWorld gameWorld) : base(gameWorld)
        {
            EndsPlayerTurn = true;
        }

        public void Initialise(IGameObject gameObject, Item item)
        {
            GameObject = gameObject;
            Item = item;
        }

        protected override CommandResult ExecuteInternal()
        {
            var map = GameObject.CurrentMap;
            var position = GameObject.Position;

            var existingItem = map.GetObjectAt<Item>(position);

            if (existingItem != null)
            {
                return Result(CommandResult.NoMove(this, "I cannot drop this item - there is another item in the way"));
            }

            Item.Position = position;

            map.AddEntity(Item);

            var itemDescription = GameWorld.Inventory.ItemTypeDiscoveries.GetInventoryDescriptionAsSingleItemLowerCase(Item);

            Item.HasBeenDropped = true;

            return Result(CommandResult.Success(this, $"I drop {itemDescription}"));
        }
    }
}
