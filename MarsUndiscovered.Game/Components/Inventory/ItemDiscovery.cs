namespace MarsUndiscovered.Game.Components
{
    public class ItemDiscovery : ICloneable
    {
        public bool IsEnchantLevelDiscovered { get; set; }
        public bool IsItemSpecialDiscovered { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public static ItemDiscovery ItemDiscoveryDiscovered = new ItemDiscovery
        {
            IsEnchantLevelDiscovered = true,
            IsItemSpecialDiscovered = true
        };
    }
}
