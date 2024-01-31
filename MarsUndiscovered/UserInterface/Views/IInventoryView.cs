using MarsUndiscovered.Game.Components;

namespace MarsUndiscovered.UserInterface.Views;

public interface IInventoryView
{
    void ClearFocussedItem(InventoryItem inventoryItem);
    void AfterItemFocussed(InventoryItem inventoryItem);
    void ClearFocus();
    void OnMouseDown(InventoryItem inventoryItem);
}