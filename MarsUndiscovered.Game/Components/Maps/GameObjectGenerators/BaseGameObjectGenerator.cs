using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Extensions;
using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Components.Maps
{
    public abstract class BaseGameObjectGenerator : BaseComponent
    {
        protected Point GetPosition<T>(T spawnGameObjectParams, MarsMap map) where T : BaseSpawnGameObjectParams
        {
            return map.RandomPositionFromRules(spawnGameObjectParams.MapPointChoiceRules, spawnGameObjectParams.Position);
        }
    }
}
