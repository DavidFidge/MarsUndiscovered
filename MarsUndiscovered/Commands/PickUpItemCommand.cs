using System.Diagnostics;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using GoRogue.GameFramework;

using MarsUndiscovered.Components;
using MarsUndiscovered.Interfaces;

using SadRogue.Primitives;

namespace MarsUndiscovered.Commands
{
    public class PickUpItemCommand : BaseMarsGameActionCommand<PickUpItemSaveData>
    {
        public Item Item { get; private set; }
        public IGameObject GameObject { get; private set; }

        public PickUpItemCommand(IGameWorld gameWorld) : base(gameWorld)
        {
        }

        public void Initialise(Item item, IGameObject gameObject)
        {
            GameObject = gameObject;
            Item = item;

            Debug.Assert(Item.Position.Equals(GameObject.Position));
        }

        public override IMemento<PickUpItemSaveData> GetSaveState()
        {
            var memento = new Memento<PickUpItemSaveData>(new PickUpItemSaveData());
            base.PopulateLoadState(memento.State);
            memento.State.ItemId = Item.ID;
            memento.State.GameObjectId = GameObject.ID;
            return memento;
        }

        public override void SetLoadState(IMemento<PickUpItemSaveData> memento)
        {
            base.PopulateLoadState(memento.State);
            GameObject = GameWorld.GameObjects[memento.State.GameObjectId];
            Item = GameWorld.Items[memento.State.ItemId];
        }

        protected override CommandResult ExecuteInternal()
        {
            var player = GameObject as Player;

            if (player == null)
            {
                return Result(CommandResult.Exception(this, "Monsters currently do not pick up items"));
            }

            var itemDescription = GameWorld.Inventory.ItemTypeDiscoveries.GetInventoryDescriptionAsSingleItemLowerCase(Item);

            if (!GameWorld.Inventory.CanPickUpItem)
            {
                return Result(CommandResult.NoMove(this, $"Your inventory is too full to pick up {itemDescription}"));
            }

            GameWorld.Inventory.Add(Item);
            Item.CurrentMap.RemoveEntity(Item);
            Item.Position = Point.None;

            return Result(CommandResult.Success(this, $"You pick up {itemDescription}"));
        }

        protected override void UndoInternal()
        {
            GameWorld.Inventory.Remove(Item);
            Item.Position = GameObject.Position;
            GameObject.CurrentMap.AddEntity(Item);
        }
    }
}
