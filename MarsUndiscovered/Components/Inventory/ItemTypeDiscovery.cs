namespace MarsUndiscovered.Components
{
    public class ItemTypeDiscovery
    {
        public bool IsItemTypeDiscovered { get; set; }
        public string UndiscoveredName { get; set; }
        public string UndiscoveredNamePrefix { get; set; }

        public ItemTypeDiscovery()
        {
        }

        public ItemTypeDiscovery(string undiscoveredName)
        {
            UndiscoveredNamePrefix = undiscoveredName.Split(' ')[0];
            UndiscoveredName = undiscoveredName.Split(' ')[1];
        }

        public static ItemTypeDiscovery ItemTypeDiscoveryDiscovered =
            new ItemTypeDiscovery { IsItemTypeDiscovered = true };
    }
}