using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Extensions;
using MarsUndiscovered.Game.ViewMessages;
using NGenerics.Sorting;

namespace MarsUndiscovered.Game.Components.Maps
{
    public class ItemGenerator : BaseGameObjectGenerator, IItemGenerator
    {
        public Item SpawnItem(SpawnItemParams spawnItemParams, IGameObjectFactory gameObjectFactory, MapCollection maps, ItemCollection itemCollection)
        {
            if (spawnItemParams.ItemType == null)
                return null;

            var item = gameObjectFactory
                .CreateGameObject<Item>()
                .WithItemType(spawnItemParams.ItemType);

            if (spawnItemParams.Inventory == null)
            {
                var map = maps.Single(m => m.Id == spawnItemParams.MapId);

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
