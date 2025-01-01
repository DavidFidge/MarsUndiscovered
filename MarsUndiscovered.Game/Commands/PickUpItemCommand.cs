using System.Diagnostics;
using FrigidRogue.MonoGame.Core.Components;
using GoRogue.GameFramework;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;
using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Commands
{
    public class PickUpItemCommand : BaseMarsGameActionCommand<PickUpItemSaveData>
    {
        public Item Item => GameWorld.Items[_data.ItemId];
        public IGameObject GameObject => GameWorld.GameObjects[_data.GameObjectId];

        public PickUpItemCommand(IGameWorld gameWorld) : base(gameWorld)
        {
        }

        public void Initialise(Item item, IGameObject gameObject)
        {
            _data.GameObjectId = gameObject.ID;
            _data.ItemId = item.ID;

            Debug.Assert(Item.Position.Equals(GameObject.Position));
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
                return Result(CommandResult.NoMove(this, $"My inventory is too full to pick up {itemDescription}"));
            }

            GameWorld.Inventory.Add(Item);
            Item.CurrentMap.RemoveEntity(Item);
            Item.Position = Point.None;

            return Result(CommandResult.Success(this, $"I pick up {itemDescription}"));
        }

        protected override void UndoInternal()
        {
            GameWorld.Inventory.Remove(Item);
            Item.Position = GameObject.Position;
            GameObject.CurrentMap.AddEntity(Item);
        }
    }
}
