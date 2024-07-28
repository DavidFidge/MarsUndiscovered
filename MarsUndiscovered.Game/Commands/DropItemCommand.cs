using FrigidRogue.MonoGame.Core.Components;

using GoRogue.GameFramework;

using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;
using Microsoft.Xna.Framework.Input;
using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Commands
{
    public class DropItemCommand : BaseMarsGameActionCommand<DropItemSaveData>
    {
        public Item Item => GameWorld.Items[_data.ItemId];
        public IGameObject GameObject => GameWorld.GameObjects[_data.GameObjectId];
        private bool _wasInInventory => _data.WasInInventory;
        private bool _wasEquipped => _data.WasEquipped;
        private bool _oldHasBeenDropped => _data.OldHasBeenDropped;
        private Keys _itemKey => _data.ItemKey;

        public DropItemCommand(IGameWorld gameWorld) : base(gameWorld)
        {
            EndsPlayerTurn = true;
        }

        public void Initialise(IGameObject gameObject, Item item)
        {
            _data.GameObjectId = gameObject.ID;
            _data.ItemId = item.ID;
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
            
            _data.ItemKey = GameWorld.Inventory.GetKeyForItem(Item);
            _data.WasEquipped = GameWorld.Inventory.IsEquipped(Item);
            _data.WasInInventory = GameWorld.Inventory.Remove(Item);

            Item.Position = position;

            map.AddEntity(Item);

            var itemDescription = GameWorld.Inventory.ItemTypeDiscoveries.GetInventoryDescriptionAsSingleItemLowerCase(Item);
            _data.OldHasBeenDropped = Item.HasBeenDropped;
            Item.HasBeenDropped = true;

            return Result(CommandResult.Success(this, $"I drop {itemDescription}"));
        }

        protected override void UndoInternal()
        {
            if (_wasInInventory)
            {
                GameWorld.Inventory.Add(Item, _itemKey);
            }

            if (_wasEquipped)
            {
                GameWorld.Inventory.Equip(Item);
            }

            Item.HasBeenDropped = _oldHasBeenDropped;

            Item.Position = Point.None;

            GameObject.CurrentMap.RemoveEntity(Item);
        }
    }
}
