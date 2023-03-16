using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Components.SaveData;

namespace MarsUndiscovered.Components
{
    public class ItemCollection : GameObjectCollection<Item, ItemSaveData>
    {
        private readonly IGameObjectFactory _gameObjectFactory;

        public ItemCollection(IGameObjectFactory gameObjectFactory)
        {
            _gameObjectFactory = gameObjectFactory;
        }

        protected override Item Create(uint id)
        {
            return _gameObjectFactory.CreateItem(id);
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