using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.MonoGame.Core.View.Extensions;

using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;

namespace MarsUndiscovered.UserInterface.Views;

public abstract class BaseInventoryView<TViewModel, TData> : BaseMarsUndiscoveredView<TViewModel, TData>
    where TViewModel : BaseInventoryViewModel<TData>
    where TData : BaseInventoryData, new()
{
    protected Panel InventoryPanel;
    protected Panel InventoryHoverPanel;
    protected Panel InventoryContainerPanel;
    protected List<InventoryItemPanel> InventoryItems;
    protected Label InventoryLabel;
    protected InventoryMode InventoryMode;
    protected RichParagraph InventoryHoverPanelText { get; set; }

    public BaseInventoryView(
        TViewModel inventoryViewModel
    ) : base(inventoryViewModel)
    {
    }

    protected override void InitializeInternal()
    {
        InventoryContainerPanel = new Panel()
            .Anchor(Anchor.CenterRight)
            .Width(Constants.MiddlePanelWidth)
            .NoSkin()
            .NoPadding()
            .Height(0.79f)
            .Offset(new Vector2(20f, 120f));

        InventoryHoverPanel = new Panel()
            .Anchor(Anchor.TopLeft)
            .Width(0.45f)
            .Skin(PanelSkin.Alternative)
            .AutoHeight()
            .Hidden();

        InventoryHoverPanelText = new RichParagraph();

        InventoryHoverPanel.AddChild(InventoryHoverPanelText);

        InventoryContainerPanel.AddChild(InventoryHoverPanel);

        InventoryPanel = new Panel()
            .Anchor(Anchor.TopRight)
            .Width(0.45f)
            .Skin(PanelSkin.Alternative)
            .AutoHeight();

        InventoryContainerPanel.AddChild(InventoryPanel);

        InventoryLabel = new Label("Your Inventory:")
            .NoPadding()
            .Anchor(Anchor.Auto);

        InventoryPanel.AddChild(InventoryLabel);

        InventoryItems = new List<InventoryItemPanel>(26);
        
        for (var i = 0; i < 26; i++)
            InventoryItems.Add(new InventoryItemPanel(InventoryPanel));

        RootPanel.AddChild(InventoryContainerPanel);
    }

    public override void Show()
    {
        base.Show();

        HidePanels();

        var inventoryItems = _viewModel.GetInventoryItems();

        if (inventoryItems.IsEmpty())
            InventoryItems[0].SetNoInventory();

        for (var i = 0; i < inventoryItems.Count; i++)
        {
            InventoryItems[i].SetInventoryItem(inventoryItems[i], this.InventoryMode);
        }
    }
    
    public void HidePanels()
    {
        foreach (var item in InventoryItems)
        {
            item.Hidden();
        }
    }
}