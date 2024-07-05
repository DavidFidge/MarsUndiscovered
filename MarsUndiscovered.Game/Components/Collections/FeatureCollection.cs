using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.SaveData;

namespace MarsUndiscovered.Game.Components
{
    public class FeatureCollection : GameObjectCollection<Feature, FeatureSaveData>
    {
        public FeatureCollection(IGameObjectFactory gameObjectFactory) : base(gameObjectFactory)
        {
        }
    }
}