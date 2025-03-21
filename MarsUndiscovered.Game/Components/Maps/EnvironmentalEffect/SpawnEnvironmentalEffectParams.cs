namespace MarsUndiscovered.Game.Components
{
    public class SpawnEnvironmentalEffectParams : BaseSpawnGameObjectParams
    {
        public EnvironmentalEffectType EnvironmentalEffectType { get; set; }
        public EnvironmentalEffect Result { get; set; }

        public SpawnEnvironmentalEffectParams WithEnvironmentalEffectType(EnvironmentalEffectType environmentalEffectType)
        {
            EnvironmentalEffectType = environmentalEffectType;
            return this;
        }
    }
}