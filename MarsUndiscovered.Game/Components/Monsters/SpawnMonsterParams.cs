namespace MarsUndiscovered.Game.Components
{
    public class SpawnMonsterParams : BaseSpawnGameObjectParams
    {
        public Breed Breed { get; set; }
        public Monster Result { get; set; }
    }

    public static class SpawnMonsterParamsFluentExtensions
    {
        public static SpawnMonsterParams WithBreed(this SpawnMonsterParams spawnMonsterParams, Breed breed)
        {
            spawnMonsterParams.Breed = breed;
            return spawnMonsterParams;
        }

        public static SpawnMonsterParams WithBreed(this SpawnMonsterParams spawnMonsterParams, string breed)
        {
            spawnMonsterParams.Breed = Breed.GetBreed(breed);
            return spawnMonsterParams;
        }
    }
}