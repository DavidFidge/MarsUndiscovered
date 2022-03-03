using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Extensions;
using MarsUndiscovered.Messages;

namespace MarsUndiscovered.Components.Maps
{
    public class ItemGenerator : BaseGameObjectGenerator, IItemGenerator
    {
        public Item SpawnItem(SpawnItemParams spawnItemParams, IGameObjectFactory gameObjectFactory, MarsMap map, ItemCollection itemCollection)
        {
            if (spawnItemParams.ItemType == null)
                return null;

            var item = gameObjectFactory
                .CreateItem()
                .WithItemType(spawnItemParams.ItemType)
                .PositionedAt(GetPosition(spawnItemParams, map))
                .AddToMap(map);

            itemCollection.Add(item.ID, item);

            Mediator.Publish(new MapTileChangedNotification(item.Position));

            return item;
        }
    }
}
