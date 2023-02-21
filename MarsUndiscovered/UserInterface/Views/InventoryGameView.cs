using System.Threading;
using System.Threading.Tasks;

using MarsUndiscovered.Components;
using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

using MediatR;

namespace MarsUndiscovered.UserInterface.Views
{
    public class InventoryGameView : BaseInventoryView<InventoryGameViewModel, InventoryGameData>,
        IRequestHandler<InventoryItemSelectionRequest>
    {

        public InventoryGameView(
            InventoryGameViewModel inventoryGameViewModel
        ) : base(inventoryGameViewModel)
        {
        }

        public void SetInventoryMode(InventoryMode inventoryMode)
        {
            InventoryMode = inventoryMode;

            switch (inventoryMode)
            {
                case Views.InventoryMode.ReadOnly:
                case Views.InventoryMode.View:
                    InventoryLabel.Text = "Your Inventory:";
                    break;
                case Views.InventoryMode.Equip:
                    InventoryLabel.Text = "Equip what?";
                    break;
                case Views.InventoryMode.Unequip:
                    InventoryLabel.Text = "Remove (unequip) what?";
                    break;
                case Views.InventoryMode.Drop:
                    InventoryLabel.Text = "Drop what?";
                    break;
            }
        }

        public Task<Unit> Handle(InventoryItemSelectionRequest request, CancellationToken cancellationToken)
        {
            switch (InventoryMode)
            {
                case Views.InventoryMode.View:
                    break;
                case Views.InventoryMode.Equip:
                    _viewModel.EquipRequest(request.Key);
                    break;
                case Views.InventoryMode.Unequip:
                    _viewModel.UnequipRequest(request.Key);
                    break;
                case Views.InventoryMode.Drop:
                    _viewModel.DropRequest(request.Key);
                    break;
                case Views.InventoryMode.ReadOnly:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return Unit.Task;
        }
    }
}