namespace MarsUndiscovered.Game.Components
{
    public class SpawnMonsterParams : BaseSpawnGameObjectParams
    {
        public Breed Breed { get; set; }
        public Monster Result { get; set; }
        public uint? LeaderId { get; set; }
        public MonsterState MonsterState { get; set; }
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
        
        public static SpawnMonsterParams WithLeader(this SpawnMonsterParams spawnMonsterParams, uint monsterLeaderId)
        {
            spawnMonsterParams.LeaderId = monsterLeaderId;
            return spawnMonsterParams;
        }
        
        public static SpawnMonsterParams WithState(this SpawnMonsterParams spawnMonsterParams, MonsterState monsterState)
        {
            spawnMonsterParams.MonsterState = monsterState;
            return spawnMonsterParams;
        }
    }
}