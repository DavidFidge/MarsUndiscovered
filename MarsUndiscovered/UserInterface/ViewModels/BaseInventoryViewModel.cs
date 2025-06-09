using FrigidRogue.MonoGame.Core.UserInterface;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;
using MarsUndiscovered.UserInterface.Data;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public abstract class BaseInventoryViewModel<T> : BaseViewModel<T>
        where T : BaseInventoryData, new()
    {
        protected List<InventoryItem> _inventoryItems;
        public IGameWorldProvider GameWorldProvider { get; set; }
        public IGameWorld GameWorld => GameWorldProvider.GameWorld;

        public List<InventoryItem> GetInventoryItems()
        {
            _inventoryItems = GameWorld
                .GetInventoryItems()
                .OrderBy(i => i.Key)
                .ToList();

            return _inventoryItems;
        }
    }
}
