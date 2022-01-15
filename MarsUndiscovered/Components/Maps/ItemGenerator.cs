using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Extensions;
using GoRogue.GameFramework;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Extensions;
using MarsUndiscovered.Messages;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Components.Maps
{
    public class ItemGenerator : BaseComponent, IItemGenerator
    {
        public IGameObjectFactory GameObjectFactory { get; set; }

        public Item SpawnItem(SpawnItemParams spawnItemParams, Map map, ItemCollection itemCollection)
        {
            if (spawnItemParams.ItemType == null)
                return null;

            var item = GameObjectFactory
                .CreateItem()
                .WithItemType(spawnItemParams.ItemType)
                .PositionedAt(GetPosition(spawnItemParams, map))
                .AddToMap(map);

            itemCollection.Add(item.ID, item);

            Mediator.Publish(new MapTileChangedNotification(item.Position));

            return item;
        }

        private Point GetPosition(SpawnItemParams spawnItemParams, Map map)
        {
            if (spawnItemParams.Position != null)
                return spawnItemParams.Position.Value;

            if (spawnItemParams.AvoidPosition != null)
                return map.RandomPositionAwayFrom(
                    spawnItemParams.AvoidPosition.Value,
                    spawnItemParams.AvoidPositionRange,
                    MapHelpers.EmptyPointOnFloor
                );

            return map.RandomPosition(MapHelpers.EmptyPointOnFloor);
        }
    }
}
