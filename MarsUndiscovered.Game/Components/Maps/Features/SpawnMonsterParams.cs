namespace MarsUndiscovered.Game.Components
{
    public class SpawnFeatureParams : BaseSpawnGameObjectParams
    {
        public FeatureType FeatureType { get; set; }
        public Feature Result { get; set; }

        public SpawnFeatureParams WithFeatureType(FeatureType featureType)
        {
            FeatureType = featureType;
            return this;
        }
    }

    public static class SpawnFeatureParamsFluentExtensions
    {
    }
}