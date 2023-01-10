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
        private InventoryMode _inventoryMode;

        public InventoryGameView(
            InventoryGameViewModel inventoryGameViewModel
        ) : base(inventoryGameViewModel)
        {
        }

        public void SetInventoryMode(InventoryMode inventoryMode)
        {
            _inventoryMode = inventoryMode;

            switch (inventoryMode)
            {
                case InventoryMode.ReadOnly:
                case InventoryMode.View:
                    _inventoryLabel.Text = "Your Inventory:";
                    break;
                case InventoryMode.Equip:
                    _inventoryLabel.Text = "Equip what?";
                    break;
                case InventoryMode.Unequip:
                    _inventoryLabel.Text = "Remove (unequip) what?";
                    break;
                case InventoryMode.Drop:
                    _inventoryLabel.Text = "Drop what?";
                    break;
            }
        }

        protected override string GetInventoryItemText(InventoryItem inventoryItem)
        {
            switch (_inventoryMode)
            {
                case InventoryMode.View:
                    return base.GetInventoryItemText(inventoryItem);
                case InventoryMode.Equip:
                    return inventoryItem.CanEquip
                        ? base.GetInventoryItemText(inventoryItem)
                        : GetGrayInventoryItemText(inventoryItem);
                case InventoryMode.Unequip:
                    return inventoryItem.CanUnequip
                        ? base.GetInventoryItemText(inventoryItem)
                        : GetGrayInventoryItemText(inventoryItem);
                case InventoryMode.Drop:
                    return inventoryItem.CanDrop
                        ? base.GetInventoryItemText(inventoryItem)
                        : GetGrayInventoryItemText(inventoryItem);
                case InventoryMode.ReadOnly:
                    return base.GetInventoryItemText(inventoryItem);
                default:
                    return base.GetInventoryItemText(inventoryItem);
            }
        }

        protected string GetGrayInventoryItemText(InventoryItem inventoryItem)
        {
            return $"{{{{GRAY}}}}{inventoryItem.KeyDescription} {inventoryItem.ItemDescription}{{{{DEFAULT}}}}";
        }

        public Task<Unit> Handle(InventoryItemSelectionRequest request, CancellationToken cancellationToken)
        {
            switch (_inventoryMode)
            {
                case InventoryMode.View:
                    break;
                case InventoryMode.Equip:
                    _viewModel.EquipRequest(request.Key);
                    break;
                case InventoryMode.Unequip:
                    _viewModel.UnequipRequest(request.Key);
                    break;
                case InventoryMode.Drop:
                    _viewModel.DropRequest(request.Key);
                    break;
                case InventoryMode.ReadOnly:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return Unit.Task;
        }
    }
}