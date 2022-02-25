using System.Collections.Generic;

namespace MarsUndiscovered.Components
{
    public abstract class Breed
    {
        public virtual string GenericArticleLowerCase => "a";
        public virtual string GenericArticleUpperCase => "A";
        public abstract string Name { get; }
        public string Description { get; set; }
        public decimal HealthModifier { get; set; }

        public bool IsWallTurret { get; set; }

        public static Dictionary<string, Breed> Breeds;
        public static Roach Roach = new Roach();
        public static TeslaCoil TeslaCoil = new TeslaCoil();
        public Attack BasicAttack { get; protected set; } = null;
        public LightningAttack LightningAttack { get; protected set; } = null;

        static Breed()
        {
            Breeds = new Dictionary<string, Breed>();

            Breeds.Add(Roach.Name, Roach);
            Breeds.Add(TeslaCoil.Name, TeslaCoil);
        }

        public static Breed GetBreed(string breed)
        {
            return Breeds[breed];
        }
    }
}