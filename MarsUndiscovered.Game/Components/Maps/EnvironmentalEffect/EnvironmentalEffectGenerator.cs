using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.Maps.MapPointChoiceRules;
using MarsUndiscovered.Game.Extensions;
using MarsUndiscovered.Game.ViewMessages;
using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Components.Maps
{
    public class EnvironmentalEffectGenerator : BaseGameObjectGenerator, IEnvironmentalEffectGenerator
    {
        public void SpawnEnvironmentalEffect(SpawnEnvironmentalEffectParams spawnEnvironmentalEffectParams, IGameObjectFactory gameObjectFactory, MapCollection maps, EnvironmentalEffectCollection EnvironmentalEffectCollection)
        {
            spawnEnvironmentalEffectParams.Result = null;

            var map = maps.Single(m => m.Id == spawnEnvironmentalEffectParams.MapId);
            
            spawnEnvironmentalEffectParams.AssignMap(map);
            
            var EnvironmentalEffect = gameObjectFactory
                .CreateGameObject<EnvironmentalEffect>()
                .WithEnvironmentalEffectType(spawnEnvironmentalEffectParams.EnvironmentalEffectType);
            
            var position = GetPosition(spawnEnvironmentalEffectParams, map);
            
            if (position == Point.None)
                return;
                
            EnvironmentalEffect.PositionedAt(position)
                .AddToMap(map);
            
            EnvironmentalEffectCollection.Add(EnvironmentalEffect.ID, EnvironmentalEffect);
            
            Mediator.Publish(new MapTileChangedNotification(EnvironmentalEffect.Position));

            spawnEnvironmentalEffectParams.Result = EnvironmentalEffect;
        }
    }
}
