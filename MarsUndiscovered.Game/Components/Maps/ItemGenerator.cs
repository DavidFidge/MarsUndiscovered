using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Extensions;
using MarsUndiscovered.Messages;

namespace MarsUndiscovered.Game.Components.Maps
{
    public class ItemGenerator : BaseGameObjectGenerator, IItemGenerator
    {
        public Item SpawnItem(SpawnItemParams spawnItemParams, IGameObjectFactory gameObjectFactory, MarsMap map, ItemCollection itemCollection)
        {
            if (spawnItemParams.ItemType == null)
                return null;

            var item = gameObjectFactory
                .CreateItem()
                .WithItemType(spawnItemParams.ItemType);

            if (spawnItemParams.Inventory == null)
            {
                item.PositionedAt(GetPosition(spawnItemParams, map))
                    .AddToMap(map);
            }
            else
            {
                spawnItemParams.Inventory.Add(item);
            }

            itemCollection.Add(item.ID, item);
            
            if (spawnItemParams.Inventory == null)
                Mediator.Publish(new MapTileChangedNotification(item.Position));

            return item;
        }
    }
}
