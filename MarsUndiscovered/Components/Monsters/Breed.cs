using System.Collections.Generic;

namespace MarsUndiscovered.Components
{
    public class Breed
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal HealthModifier { get; set; }

        public static Dictionary<string, Breed> Breeds;

        static Breed()
        {
            Breeds = new Dictionary<string, Breed>();

            var roach = new Roach();

            Breeds.Add(roach.Name, roach);
        }
    }
}