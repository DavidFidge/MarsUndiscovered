namespace MarsUndiscovered.Game.Components
{
    public class SpawnItemParams : BaseSpawnGameObjectParams
    {
        public ItemType ItemType { get; set; }
        public Inventory Inventory { get; set; }
        public bool IntoPlayerInventory { get; set; }
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

        public static SpawnItemParams InInventory(this SpawnItemParams spawnItemParams, Inventory inventory)
        {
            spawnItemParams.Inventory = inventory;
            return spawnItemParams;
        }
        
        public static SpawnItemParams IntoPlayerInventory(this SpawnItemParams spawnItemParams, bool intoPlayerInventory = true)
        {
            spawnItemParams.IntoPlayerInventory = intoPlayerInventory;
            return spawnItemParams;
        }
    }
}