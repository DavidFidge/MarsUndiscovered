using FrigidRogue.MonoGame.Core.View.Extensions;
using GeonBit.UI.Entities;
using MarsUndiscovered.Components;
using MarsUndiscovered.Graphics;
using MarsUndiscovered.Interfaces;
using Microsoft.Xna.Framework;

namespace MarsUndiscovered.UserInterface.Views;

public class InventoryItemPanel : Panel
{
    private readonly IInventoryView _inventoryView;
    private readonly IAssets _assets;
    public InventoryItem InventoryItem => _inventoryItem;
    public bool HasFocus => _hasFocus;

    private Paragraph _key;
    private Paragraph _description;
    private InventoryItem _inventoryItem;
    private ColoredRectangle _coloredRectangle;
    private Color _backgroundColour;
    private bool _hasFocus = false;
    private Image _itemImage;

    public InventoryItemPanel(IInventoryView inventoryView, IAssets assets)
    {
        _inventoryView = inventoryView;
        _assets = assets;

        this.Anchor(Anchor.Auto)
            .SkinNone()
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
        
        _itemImage = new Image()
            .Anchor(Anchor.AutoInlineNoBreak)
            .Size(0, 0)
            .NoPadding();
        
        AddChild(_itemImage);
        
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
        _inventoryView.OnMouseDown(_inventoryItem);
    }

    public void OnInventoryPanelEnter()
    {
        SetFocus();
    }
    
    public void OnInventoryPanelLeave()
    {
        ClearFocus();
    }
    
    private void OnInventoryItemPanelMouseEnter(Entity entity)
    {
        SetFocus();
    }

    private void ClearFocus()
    {
        _hasFocus = false;
        _coloredRectangle.FillColor(Color.Black);
        _inventoryView.ClearFocussedItem(_inventoryItem);
    }

    private void SetFocus()
    {
        _inventoryView.ClearFocus();

        if (_inventoryItem != null) // Omit setting focus if no inventory item e.g. when slot is showing "Your pack is empty"
        {
            _hasFocus = true;
            _coloredRectangle.FillColor(_backgroundColour);
            _inventoryView.SetFocussedItem(_inventoryItem);
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

        _itemImage.Texture = _assets.GetStaticTexture(_inventoryItem.ItemType);
        _itemImage.Size(_itemImage.Texture.Width, _itemImage.Texture.Height);

        _description.Text = _inventoryItem.ItemDescription;
        _description.Offset(_itemImage.Texture.Width, 0);

        _description.FillColor(Color.White);
        _description.RecalculateWidth();

        if ((inventoryMode == InventoryMode.Equip && !inventoryItem.CanEquip) ||
            (inventoryMode == InventoryMode.Unequip && !inventoryItem.CanUnequip) ||
            (inventoryMode == InventoryMode.Drop && !inventoryItem.CanDrop) ||
            (inventoryMode == InventoryMode.Apply && !inventoryItem.CanApply) ||
            inventoryMode == InventoryMode.ReadOnly)
        {
            _key.FillColor(Color.Gray);
            _description.FillColor(Color.Gray);
        }
        
        this.Visible();
    }

    public void SetNoInventory()
    {
        _hasFocus = false;
        _coloredRectangle.FillColor(Color.Black);
        _key.Text = "Your pack is empty";
        _key.CalcTextActualRectWithWrap();
        this.Height(_key.GetTextDestRect().Height);
        _description.Text = String.Empty;
        _description.Offset(0, 0);
        _itemImage.Texture = null;
        _itemImage.Size(0, 0);
        this.Visible();
    }

    public void ClearAndHide()
    {
        this.Hidden();
        _inventoryItem = null;
        _hasFocus = false;
    }
}