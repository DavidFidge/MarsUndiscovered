using FrigidRogue.MonoGame.Core.View.Extensions;
using GeonBit.UI.Entities;
using MarsUndiscovered.Components;
using Microsoft.Xna.Framework;

namespace MarsUndiscovered.UserInterface.Views;

public class InventoryItemPanel : Panel
{
    private Paragraph _key;
    private Paragraph _description;
    private InventoryItem _inventoryItem;

    public InventoryItemPanel(Panel inventoryContainerPanel)
    {
        this.Anchor(Anchor.Auto)
            .SimpleSkin()
            .NoPadding()
            .Height(1);

        _key = new Paragraph()
            .Anchor(Anchor.AutoInlineNoBreak)
            .NoPadding()
            .NoWrap()
            .WidthOfContainer();

        AddChild(_key);
        
        _description = new Paragraph()
            .Anchor(Anchor.AutoInlineNoBreak)
            .NoPadding()
            .NoWrap()
            .WidthOfContainer();

        AddChild(_description);

        inventoryContainerPanel.AddChild(this);
    }

    public void SetInventoryItem(InventoryItem inventoryItem, InventoryMode inventoryMode)
    {
        _inventoryItem = inventoryItem;

        _key.Text = _inventoryItem.KeyDescription;
        _key.FillColor(Color.White);
        _key.RecalculateWidth();
        
        _key.CalcTextActualRectWithWrap();
        this.Height(_key.GetTextDestRect().Height);

        _description.Text = _inventoryItem.ItemDescription;
        _description.FillColor(Color.White);
        _description.RecalculateWidth();

        if ((inventoryMode == InventoryMode.Equip && !inventoryItem.CanEquip) ||
            (inventoryMode == InventoryMode.Unequip && !inventoryItem.CanUnequip) ||
            (inventoryMode == InventoryMode.Drop && !inventoryItem.CanDrop) ||
            inventoryMode == InventoryMode.ReadOnly)
        {
            _key.FillColor(Color.Gray);
            _description.FillColor(Color.Gray);
        }
        
        this.Visible();
    }

    public void SetNoInventory()
    {
        _key.Text = "Your pack is empty";
        _key.CalcTextActualRectWithWrap();
        this.Height(_key.GetTextDestRect().Height);

        _description.Text = String.Empty;
        this.Visible();
    }
}