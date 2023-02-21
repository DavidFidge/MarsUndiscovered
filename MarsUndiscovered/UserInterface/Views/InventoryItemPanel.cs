using FrigidRogue.MonoGame.Core.View.Extensions;
using GeonBit.UI.Entities;
using MarsUndiscovered.Components;
using MarsUndiscovered.Graphics;
using Microsoft.Xna.Framework;

namespace MarsUndiscovered.UserInterface.Views;

public class InventoryItemPanel : Panel
{
    private Paragraph _key;
    private Paragraph _description;
    private InventoryItem _inventoryItem;
    private ColoredRectangle _coloredRectangle;
    private Color _backgroundColour;

    public InventoryItemPanel(Panel inventoryContainerPanel)
    {
        this.Anchor(Anchor.Auto)
            .NoSkin()
            .NoPadding()
            .WidthOfContainer()
            .Height(1);

        _backgroundColour = Assets.UserInterfaceColor * (100f / 255.0f);
        
        _coloredRectangle = new ColoredRectangle(_backgroundColour)
            .Anchor(Anchor.Center)
            .NoPadding()
            .Offset(new Vector2(0, 0));
                
        Background = _coloredRectangle;
        
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
        
        OnMouseLeave += OnInventoryPanelMouseLeave;
        WhileMouseHover += OnInventoryPanelMouseEnter;

        _key.WhileMouseHover += OnInventoryPanelMouseEnter;
        _key.OnMouseLeave += OnInventoryPanelMouseLeave;
     
        _description.WhileMouseHover += OnInventoryPanelMouseEnter;
        _description.OnMouseLeave += OnInventoryPanelMouseLeave;
    }

    private void OnInventoryPanelMouseLeave(Entity entity)
    {
        _coloredRectangle.FillColor(Color.Black);
    }

    private void OnInventoryPanelMouseEnter(Entity entity)
    {
        _coloredRectangle.FillColor(_backgroundColour);
    }

    public void SetInventoryItem(InventoryItem inventoryItem, InventoryMode inventoryMode)
    {
        _coloredRectangle.FillColor(Color.Black);
        _inventoryItem = inventoryItem;

        _key.Text = _inventoryItem.KeyDescription;
        _key.FillColor(Color.White);
        _key.RecalculateWidth(20);
        
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
        _coloredRectangle.FillColor(Color.Black);
        _key.Text = "Your pack is empty";
        _key.CalcTextActualRectWithWrap();
        this.Height(_key.GetTextDestRect().Height);

        _description.Text = String.Empty;
        this.Visible();
    }
}