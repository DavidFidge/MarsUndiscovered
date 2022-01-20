namespace MarsUndiscovered.Components
{
    public class SpawnItemParams : BaseSpawnGameObject
    {
        public ItemType ItemType { get; set; }
    }

    public static class SpawnItemParamsFluentExtensions
    {
        public static SpawnItemParams WithItemType(this SpawnItemParams spawnItemParams, ItemType itemType)
        {
            spawnItemParams.ItemType = itemType;
            return spawnItemParams;
        }

        public static SpawnItemParams WithItemType(this SpawnItemParams spawnItemParams, string breed)
        {
            spawnItemParams.ItemType = ItemType.GetItemType(breed);
            return spawnItemParams;
        }
    }
}