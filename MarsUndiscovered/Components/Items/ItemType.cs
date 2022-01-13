using System;
using System.Collections.Generic;

using GoRogue.Components;

namespace MarsUndiscovered.Components
{
    public abstract class ItemType
    {
        public static Dictionary<string, ItemType> ItemTypes;
        public static ShieldGenerator ShieldGenerator = new ShieldGenerator();
        public static MagnesiumPipe MagnesiumPipe = new MagnesiumPipe();
        public abstract void ApplyProperties(Item item);

        public virtual void ApplyEnchantmentLevel(Item item)
        {
            item.EnchantmentLevel = 0;
        }

        public abstract string Name { get; }

        static ItemType()
        {
            ItemTypes = new Dictionary<string, ItemType>();

            ItemTypes.Add(ShieldGenerator.Name.ToLower(), ShieldGenerator);
            ItemTypes.Add(MagnesiumPipe.Name.ToLower(), MagnesiumPipe);
        }

        public static ItemType GetItemType(string itemType)
        {
            return ItemTypes[itemType.ToLower()];
        }
    }
}