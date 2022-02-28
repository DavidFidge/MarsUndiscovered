using FrigidRogue.MonoGame.Core.Extensions;

namespace MarsUndiscovered.Components
{
    public class TeslaCoil : Breed
    {
        public override string Name => nameof(TeslaCoil).AddSpaces();

        public TeslaCoil()
        {
            Description =
                "This coil shoots lightning bolts at nearby threats.";
            HealthModifier = 0.5m;

            LightningAttack = new LightningAttack(5);
            IsWallTurret = true;
        }
    }
}