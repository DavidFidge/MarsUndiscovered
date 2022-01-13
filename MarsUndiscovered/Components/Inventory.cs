using System;
using System.Collections.Generic;
using System.Text;

using MarsUndiscovered.Components.Factories;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Components
{
    public class Inventory : ItemCollection
    {
        public Dictionary<Keys, Item> Items { get; set; } = new Dictionary<Keys, Item>();
        public Dictionary<Item, string> CallItem { get; set; } = new Dictionary<Item, string>();
        public Dictionary<ItemType, string> CallItemClass { get; set; } = new Dictionary<ItemType, string>();

        public Inventory(IGameObjectFactory gameObjectFactory) : base(gameObjectFactory)
        {
        }
    }
}
