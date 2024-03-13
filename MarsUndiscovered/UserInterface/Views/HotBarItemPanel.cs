using FrigidRogue.MonoGame.Core.View.Extensions;
using GeonBit.UI.Entities;
using InputHandlers.Keyboard;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Graphics;
using MarsUndiscovered.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Panel = GeonBit.UI.Entities.Panel;

namespace MarsUndiscovered.UserInterface.Views;

public class HotBarItemPanel : Panel
{
    private readonly IAssets _assets;
    public InventoryItem InventoryItem => _inventoryItem;
    
    public Keys Key { get; private set; } 
    private Paragraph _keyText;
    private Paragraph _description;
    private InventoryItem _inventoryItem;
    private Color _backgroundColour;
    private Image _itemImage;
    private ColoredRectangle _cooldownOverlay;

    public HotBarItemPanel(IAssets assets, Keys key)
    {
        _assets = assets;

        this.Anchor(Anchor.Auto)
            .SkinNone()
            .NoPadding()
            .WidthOfContainer()
            .Height(1);

        _keyText = new Paragraph()
            .Anchor(Anchor.AutoInlineNoBreak)
            .NoPadding()
            .NoWrap()
            .Offset(10, 5);
        
        _keyText.Text = key.Display(KeyboardModifier.None);
        _keyText.FillColor(Color.White);

        AddChild(_keyText);

        Key = key;

        var spacing = new Panel()
            .NoPadding()
            .SkinNone()
            .Anchor(Anchor.AutoInlineNoBreak)
            .Width(20);

        AddChild(spacing);
        
        _description = new Paragraph()
            .Anchor(Anchor.AutoInlineNoBreak)
            .NoWrap()
            .WidthOfContainer();

        AddChild(_description);

        _itemImage = new Image()
            .Anchor(Anchor.Center)
            .NoPadding();
        
        AddChild(_itemImage);
        
        _backgroundColour = Assets.UserInterfaceColor * (100f / 255.0f);
        
        // Not used yet
        _cooldownOverlay = new ColoredRectangle(_backgroundColour)
            .Anchor(Anchor.Center)
            .NoPadding()
            .Offset(new Vector2(0, 0));

        _keyText.RecalculateWidth();
        
        OnMouseEnter += OnHotBarItemPanelMouseEnter;
        OnMouseDown += OnHotBarItemPanelMouseDown;
        OnMouseLeave += OnHotBarItemPanelMouseLeave;
    }

    private void OnHotBarItemPanelMouseLeave(Entity entity)
    {
        // todo - hover leave
    }

    private void OnHotBarItemPanelMouseDown(Entity entity)
    {
        // todo trigger effect
    }
    
    private void OnHotBarItemPanelMouseEnter(Entity entity)
    {
        // todo - hover effect
    }

    public void SetInventoryItem(InventoryItem inventoryItem)
    {
        if (_inventoryItem?.ItemId == inventoryItem.ItemId)
        {
            // Item is same, no need to do any updates for now
        }
        else
        {
            _inventoryItem = inventoryItem;
            _itemImage.Texture = _assets.GetStaticTexture(_inventoryItem.ItemType.GetAbstractTypeName());
            _itemImage.Size(0, 0);
            var rect = _itemImage.CalcDestRect();
            _itemImage.Size(rect.Width * 0.5f, rect.Height * 0.5f);
            _itemImage.Offset(0, rect.Height * 0.15f);
            _description.Text = inventoryItem.HotBarDescription;
        }
    }

    public void SetNoInventory()
    {
        _description.Text = String.Empty;
        _description.Offset(0, 0);
        _itemImage.Texture = null;
        _itemImage.Size(0, 0);
        _inventoryItem = null;
    }
}