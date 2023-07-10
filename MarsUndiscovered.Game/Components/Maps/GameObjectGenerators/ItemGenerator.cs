using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.Maps.MapPointChoiceRules;
using MarsUndiscovered.Game.Extensions;
using MarsUndiscovered.Game.ViewMessages;

namespace MarsUndiscovered.Game.Components.Maps
{
    public class ItemGenerator : BaseGameObjectGenerator, IItemGenerator
    {
        public void SpawnItem(SpawnItemParams spawnItemParams, IGameObjectFactory gameObjectFactory, MapCollection maps, ItemCollection itemCollection)
        {
            spawnItemParams.Result = null;
            
            if (spawnItemParams.ItemType == null)
                return;

            var item = gameObjectFactory
                .CreateGameObject<Item>()
                .WithItemType(spawnItemParams.ItemType);

            if (spawnItemParams.Inventory == null)
            {
                var map = maps.Single(m => m.Id == spawnItemParams.MapId);
                spawnItemParams.MapPointChoiceRules.Add(new EmptyFloorRule());
                spawnItemParams.AssignMap(map);

                var position = GetPosition(spawnItemParams, map);
                
                item.PositionedAt(position)
                    .AddToMap(map);
            }
            else
            {
                spawnItemParams.Inventory.Add(item);
            }

            itemCollection.Add(item.ID, item);
            
            if (spawnItemParams.Inventory == null)
                Mediator.Publish(new MapTileChangedNotification(item.Position));

            spawnItemParams.Result = item;
        }
    }
}
