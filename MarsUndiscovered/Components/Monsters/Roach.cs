using MonoGame.Extended;

namespace MarsUndiscovered.Components
{
    public class Roach : Breed
    {
        public override string Name => nameof(Roach);

        public Roach()
        {
            Description =
                "The Mars Roach can survive days in the natural outdoors of Mars and is a master scavenger within the crooks of human dwellings. This one is the size of a human foot.";
            HealthModifier = 0.5m;

            BasicAttack = new Attack(new Range<int>(3, 6));
        }
    }
}