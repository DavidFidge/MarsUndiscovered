using MarsUndiscovered.Game.Components;

namespace MarsUndiscovered.UserInterface.Views;

public interface IInventoryView
{
    void ClearFocussedItem(InventoryItem inventoryItem);
    void SetFocussedItem(InventoryItem inventoryItem);
    void ClearFocus();
    void OnMouseDown(InventoryItem inventoryItem);
}