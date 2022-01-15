using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

namespace MarsUndiscovered.UserInterface.Views
{
    public class InventoryGameView : BaseInventoryView<InventoryGameViewModel, InventoryGameData>
    {
        public InventoryGameView(
            InventoryGameViewModel inventoryGameViewModel
        ) : base(inventoryGameViewModel)
        {
        }
    }
}