using System.Collections.Generic;

namespace MarsUndiscovered.Components
{
    public abstract class Breed
    {
        public virtual string GenericArticle => "a";
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal HealthModifier { get; set; }

        public static Dictionary<string, Breed> Breeds;
        public static Roach Roach = new Roach();
        public abstract Attack BasicAttack { get; }

        static Breed()
        {
            Breeds = new Dictionary<string, Breed>();

            Breeds.Add(Roach.Name.ToLower(), Roach);
        }

        public static Breed GetBreed(string breed)
        {
            return Breeds[breed.ToLower()];
        }
    }
}