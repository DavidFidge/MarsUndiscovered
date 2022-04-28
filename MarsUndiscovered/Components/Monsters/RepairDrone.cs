using MonoGame.Extended;

namespace MarsUndiscovered.Components
{
    public class RepairDrone : Breed
    {
        public override string Name => nameof(RepairDrone);

        public RepairDrone()
        {
            Description =
                "The repair drone is a small robot that performs minor precision repair jobs around the mine site. This droid has malfunctioned (or has been reprogrammed?) and now seeks and destroys anything it comes across. Repair drones prefer to hunt in packs and can repair each other when damaged.";
            HealthModifier = 0.5m;

            BasicAttack = new Attack(new Range<int>(5, 10));
        }
    }
}