using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

namespace MarsUndiscovered.UserInterface.Views
{
    public class InventoryReplayView : BaseInventoryView<InventoryReplayViewModel, InventoryReplayData>
    {
        public InventoryReplayView(
            InventoryReplayViewModel inventoryReplayViewModel
        ) : base(inventoryReplayViewModel)
        {
        }
    }
}