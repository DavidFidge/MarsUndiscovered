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

        public void RechargeItems()
        {
            foreach (var item in this.Values)
            {
                
            }
        }
    }
}