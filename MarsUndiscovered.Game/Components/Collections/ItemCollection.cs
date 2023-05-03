using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.SaveData;

namespace MarsUndiscovered.Game.Components
{
    public class ItemCollection : GameObjectCollection<Item, ItemSaveData>
    {
        public ItemCollection(IGameObjectFactory gameObjectFactory) : base(gameObjectFactory)
        {
        }

        public List<Item> RechargeItems()
        {
            var rechargedItems = new List<Item>();
            
            foreach (var item in Values)
            {
                var oldRechargeDelay = item.CurrentRechargeDelay;
                item.Recharge();
                
                if (oldRechargeDelay != 0  && item.CurrentRechargeDelay == 0)
                    rechargedItems.Add(item);
            }

            return rechargedItems;
        }
    }
}