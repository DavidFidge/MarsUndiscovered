using System.Diagnostics;
using FrigidRogue.MonoGame.Core.Components;
using GoRogue.GameFramework;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;
using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Commands
{
    public class PickUpItemCommand : BaseMarsGameActionCommand
    {
        public Item Item { get; set; }
        public IGameObject GameObject { get; set; }

        public PickUpItemCommand(IGameWorld gameWorld) : base(gameWorld)
        {
        }

        public void Initialise(Item item, IGameObject gameObject)
        {
            Item = item;
            GameObject = gameObject;

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
    }
}
