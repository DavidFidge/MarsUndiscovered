using MarsUndiscovered.Game.Components.Factories;

namespace MarsUndiscovered.Game.Components.Maps;

public interface IEnvironmentalEffectGenerator
{
    void SpawnEnvironmentalEffect(SpawnEnvironmentalEffectParams spawnEnvironmentalEffectParams, IGameObjectFactory gameObjectFactory, MapCollection maps, EnvironmentalEffectCollection EnvironmentalEffectCollection);
}