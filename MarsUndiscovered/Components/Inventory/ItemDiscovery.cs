using System;

namespace MarsUndiscovered.Components
{
    public class ItemDiscovery : ICloneable
    {
        public bool IsEnchantLevelDiscovered { get; set; }
        public bool IsItemSpecialDiscovered { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}