using System.Collections.Generic;
using System.Linq;

using MarsUndiscovered.UserInterface.Data;

using FrigidRogue.MonoGame.Core.UserInterface;

using MarsUndiscovered.Components;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public abstract class BaseInventoryViewModel<T> : BaseViewModel<T>
        where T : BaseInventoryData, new()
    {
        protected List<InventoryItem> _inventoryItems;
        public IGameWorldProvider GameWorldProvider { get; set; }

        public List<InventoryItem> GetInventoryItems()
        {
            _inventoryItems = GameWorldProvider.GameWorld
                .GetInventoryItems()
                .OrderBy(i => i.Key)
                .ToList();

            return _inventoryItems;
        }
    }
}