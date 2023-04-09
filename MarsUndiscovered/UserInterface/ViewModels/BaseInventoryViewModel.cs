using MarsUndiscovered.UserInterface.Data;

using FrigidRogue.MonoGame.Core.UserInterface;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public abstract class BaseInventoryViewModel<T> : BaseViewModel<T>
        where T : BaseInventoryData, new()
    {
        protected List<InventoryItem> _inventoryItems;
        public IGameWorldEndpoint GameWorldEndpoint { get; set; }

        public List<InventoryItem> GetInventoryItems()
        {
            _inventoryItems = GameWorldEndpoint
                .GetInventoryItems()
                .OrderBy(i => i.Key)
                .ToList();

            return _inventoryItems;
        }
    }
}
