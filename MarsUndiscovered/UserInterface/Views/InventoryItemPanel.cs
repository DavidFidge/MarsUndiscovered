using FrigidRogue.MonoGame.Core.View.Extensions;
using GeonBit.UI.Entities;
using MarsUndiscovered.Components;
using MarsUndiscovered.Graphics;
using Microsoft.Xna.Framework;

namespace MarsUndiscovered.UserInterface.Views;

public class InventoryItemPanel : Panel
{
    private readonly IInventoryView _inventoryView;
    public InventoryItem InventoryItem => _inventoryItem;
    public bool HasContext => _hasContext;

    private Paragraph _key;
    private Paragraph _description;
    private InventoryItem _inventoryItem;
    private ColoredRectangle _coloredRectangle;
    private Color _backgroundColour;
    private bool _hasContext = false;

    public InventoryItemPanel(IInventoryView inventoryView)
    {
        _inventoryView = inventoryView;
        
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

        OnMouseEnter += OnInventoryItemPanelMouseEnter;
        OnMouseDown += OnInventoryItemPanelMouseDown;

        _key.OnMouseEnter += OnInventoryItemPanelMouseEnter;
        _key.OnMouseDown += OnInventoryItemPanelMouseDown;
     
        _description.OnMouseEnter += OnInventoryItemPanelMouseEnter;
        _description.OnMouseDown += OnInventoryItemPanelMouseDown;
    }

    private void OnInventoryItemPanelMouseDown(Entity entity)
    {
        _inventoryView.PerformClick(_inventoryItem);
    }

    public void OnInventoryPanelEnter()
    {
        SetContext();
    }
    
    public void OnInventoryPanelLeave()
    {
        ClearContext();
    }
    
    private void OnInventoryItemPanelMouseEnter(Entity entity)
    {
        SetContext();
    }

    private void ClearContext()
    {
        _hasContext = false;
        _coloredRectangle.FillColor(Color.Black);
        _inventoryView.HideDescription(_inventoryItem);
    }

    private void SetContext()
    {
        _inventoryView.ClearExistingContext();

        if (_inventoryItem != null) // Omit setting context if no inventory item e.g. when slot is showing "Your pack is empty"
        {
            _hasContext = true;
            _coloredRectangle.FillColor(_backgroundColour);
            _inventoryView.ShowDescription(_inventoryItem);
        }
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
        _hasContext = false;
        _coloredRectangle.FillColor(Color.Black);
        _key.Text = "Your pack is empty";
        _key.CalcTextActualRectWithWrap();
        this.Height(_key.GetTextDestRect().Height);
        _description.Text = String.Empty;
        this.Visible();
    }

    public void ClearAndHide()
    {
        this.Hidden();
        _inventoryItem = null;
        _hasContext = false;
    }
}