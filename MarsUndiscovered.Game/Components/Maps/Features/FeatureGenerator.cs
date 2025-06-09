using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.Maps.MapPointChoiceRules;
using MarsUndiscovered.Game.Extensions;
using MarsUndiscovered.Game.ViewMessages;
using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Components.Maps
{
    public interface IFeatureGenerator
    {
        void SpawnFeature(SpawnFeatureParams spawnFeatureParams, IGameObjectFactory gameObjectFactory, MapCollection maps, FeatureCollection featureCollection);
    }

    public class FeatureGenerator : BaseGameObjectGenerator, IFeatureGenerator
    {
        public void SpawnFeature(SpawnFeatureParams spawnFeatureParams, IGameObjectFactory gameObjectFactory, MapCollection maps, FeatureCollection featureCollection)
        {
            spawnFeatureParams.Result = null;
            var map = maps.Single(m => m.Id == spawnFeatureParams.MapId);
            
            spawnFeatureParams.MapPointChoiceRules.Add(new NoGameObjectTypeRule<Feature>());

            spawnFeatureParams.AssignMap(map);

            var feature = gameObjectFactory
                .CreateGameObject<Feature>()
                .WithFeatureType(spawnFeatureParams.FeatureType);
            
            var position = GetPosition(spawnFeatureParams, map);
            
            if (position == Point.None)
                return;
                
            feature.PositionedAt(position)
                .AddToMap(map);
            
            featureCollection.Add(feature.ID, feature);
            
            Mediator.Publish(new MapTileChangedNotification(feature.Position));

            spawnFeatureParams.Result = feature;
        }
    }
}
