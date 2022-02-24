using MonoGame.Extended;

namespace MarsUndiscovered.Components
{
    public class TeslaCoil : Breed
    {
        public override string Name => nameof(TeslaCoil);

        public TeslaCoil()
        {
            Description =
                "This coil shoots lightning bolts at nearby threats.";
            HealthModifier = 0.5m;

            LightningAttack = new LightningAttack(10);
        }
    }
}