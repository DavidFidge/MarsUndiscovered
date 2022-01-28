using AutoMapper;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using GoRogue.GameFramework;

using MarsUndiscovered.Components;
using MarsUndiscovered.Interfaces;

using SadRogue.Primitives;

namespace MarsUndiscovered.Commands
{
    public class DropItemCommand : BaseMarsGameActionCommand<DropItemSaveData>
    {
        public Item Item { get; private set; }
        public IGameObject GameObject { get; private set; }
        private bool _wasInInventory;
        private bool _wasEquipped;
        private bool _oldHasBeenDropped;

        public DropItemCommand(IGameWorld gameWorld) : base(gameWorld)
        {
        }

        public void Initialise(IGameObject gameObject, Item item)
        {
            GameObject = gameObject;
            Item = item;
        }

        public override IMemento<DropItemSaveData> GetSaveState()
        {
            var memento = new Memento<DropItemSaveData>();
            base.PopulateSaveState(memento.State);
            memento.State.GameObjectId = GameObject.ID;
            memento.State.ItemId = Item.ID;
            return memento;
        }

        public override void SetLoadState(IMemento<DropItemSaveData> memento)
        {
            base.PopulateLoadState(memento.State);
            GameObject = GameWorld.GameObjects[memento.State.GameObjectId];
            Item = GameWorld.Items[memento.State.ItemId];
        }

        protected override CommandResult ExecuteInternal()
        {
            var map = GameObject.CurrentMap;
            var position = GameObject.Position;

            var existingItem = map.GetObjectAt<Item>(position);

            if (existingItem != null)
            {
                return Result(CommandResult.NoMove(this, "Cannot drop item - there is another item in the way"));
            }

            _wasEquipped = GameWorld.Inventory.IsEquipped(Item);
            _wasInInventory = GameWorld.Inventory.Remove(Item);

            Item.Position = position;

            map.AddEntity(Item);

            var itemDescription = GameWorld.Inventory.GetInventoryDescriptionAsSingleItemLowerCase(Item);
            _oldHasBeenDropped = Item.HasBeenDropped;
            Item.HasBeenDropped = true;

            return Result(CommandResult.Success(this, $"You drop {itemDescription}"));
        }

        protected override void UndoInternal()
        {
            if (_wasInInventory)
            {
                GameWorld.Inventory.Add(Item);
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
