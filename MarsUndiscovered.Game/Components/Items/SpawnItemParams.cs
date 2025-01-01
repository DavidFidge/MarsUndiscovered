using ShaiRandom.Collections;

namespace MarsUndiscovered.Game.Components
{
    public class SpawnItemParams : BaseSpawnGameObjectParams
    {
        public ItemType ItemType { get; set; }
        public Inventory Inventory { get; set; }
        public bool IntoPlayerInventory { get; set; }
        public Item Result { get; set; }
        public int EnchantmentLevel { get; set; }
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

        public static SpawnItemParams WithEnchantmentLevel(this SpawnItemParams spawnItemParams, int enchantmentLevel)
        {
            spawnItemParams.EnchantmentLevel = enchantmentLevel;

            return spawnItemParams;
        }
        
        public static SpawnItemParams WithRandomEnchantmentLevel(this SpawnItemParams spawnItemParams)
        {
            var weights = new List<(int item, double weight)>
            {
                (-4, 5),
                (-3, 20),
                (-2, 75),
                (-1, 100),
                (0, 500),
                (1, 200),
                (2, 100),
                (3, 50),
                (4, 20),
                (5, 10),
                (6, 1)
            };
            
            var probabilityTable = new ProbabilityTable<int>(weights);

            spawnItemParams.EnchantmentLevel = probabilityTable.NextItem();

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