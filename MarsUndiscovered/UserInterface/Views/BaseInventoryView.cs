using System.Threading;
using System.Threading.Tasks;
using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.MonoGame.Core.View.Extensions;

using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

using GeonBit.UI.Entities;
using MarsUndiscovered.Components;
using MarsUndiscovered.Messages;
using MediatR;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Views;

public interface IInventoryView
{
    void HideDescription(InventoryItem inventoryItem);
    void ShowDescription(InventoryItem inventoryItem);
    void ClearExistingContext();
    void PerformClick(InventoryItem inventoryItem);
}

public abstract class BaseInventoryView<TViewModel, TData> : BaseMarsUndiscoveredView<TViewModel, TData>, IInventoryView, IRequestHandler<CloseGameInventoryContextRequest>
    where TViewModel : BaseInventoryViewModel<TData>
    where TData : BaseInventoryData, new()
{
    protected Panel InventoryPanel;
    protected Panel InventoryItemDescriptionPanel;
    protected Panel InventoryContainerPanel;
    protected List<InventoryItemPanel> InventoryItems;
    protected Label InventoryLabel;
    protected InventoryMode InventoryMode;
    protected RichParagraph InventoryItemDescriptionPanelText { get; set; }

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

        InventoryItemDescriptionPanel = new Panel()
            .Anchor(Anchor.TopLeft)
            .Width(0.5f)
            .Skin(PanelSkin.Alternative)
            .AutoHeight()
            .Hidden();

        InventoryItemDescriptionPanelText = new RichParagraph();

        InventoryItemDescriptionPanel.AddChild(InventoryItemDescriptionPanelText);

        InventoryContainerPanel.AddChild(InventoryItemDescriptionPanel);

        InventoryPanel = new Panel()
            .Anchor(Anchor.TopRight)
            .Width(0.5f)
            .Skin(PanelSkin.Alternative)
            .AutoHeight();

        InventoryContainerPanel.AddChild(InventoryPanel);

        InventoryLabel = new Label("Your Inventory:")
            .NoPadding()
            .Anchor(Anchor.Auto);

        InventoryPanel.AddChild(InventoryLabel);

        // Pre-create all the inventory panels which will be visible or hidden depending on items the player actually has.
        // The player's inventory is currently limited to 26 i.e. the keys a-z.
        InventoryItems = new List<InventoryItemPanel>(26);

        for (var i = 0; i < 26; i++)
        {
            var inventoryItemPanel = new InventoryItemPanel(this);
            InventoryItems.Add(inventoryItemPanel);
            InventoryPanel.AddChild(inventoryItemPanel);
        }

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
        InventoryItemDescriptionPanel.Hidden();
        
        foreach (var item in InventoryItems)
        {
            item.ClearAndHide();
        }
    }

    protected void PerformKeyAction(Keys requestKey)
    {
        var contextItem = InventoryItems.FirstOrDefault(i => i.HasContext);

        if (contextItem != null)
        {
            PerformContextualKeyAction(contextItem.InventoryItem, requestKey);
        }
        else
        {
            var item = InventoryItems.FirstOrDefault(i => i.InventoryItem?.Key == requestKey);

            if (item != null)
            {
                foreach (var inventoryItemPanel in InventoryItems)
                {
                    inventoryItemPanel.OnInventoryPanelLeave();
                }

                item.OnInventoryPanelEnter();
            }
        }
    }

    protected virtual void PerformContextualKeyAction(InventoryItem contextItem, Keys requestKey)
    {
    }

    public void HideDescription(InventoryItem inventoryItem)
    {
        InventoryItemDescriptionPanel.Hidden();
    }

    public void ShowDescription(InventoryItem inventoryItem)
    {
        if (inventoryItem != null)
        {
            InventoryItemDescriptionPanelText.Text =
                $"{inventoryItem.ItemDescription}\n{inventoryItem.LongDescription}";
            InventoryItemDescriptionPanel.Visible();
        }
    }

    public void ClearExistingContext()
    {
        foreach (var inventoryItemPanel in InventoryItems)
        {
            inventoryItemPanel.OnInventoryPanelLeave();
        }
    }

    public void PerformClick(InventoryItem inventoryItem)
    {
        PerformInventoryModeAction(inventoryItem);
    }

    protected virtual void PerformInventoryModeAction(InventoryItem contextItem)
    {
    }

    public Task<Unit> Handle(CloseGameInventoryContextRequest request, CancellationToken cancellationToken)
    {
        if (!IsVisible)
            return Unit.Task;
        
        var contextItem = InventoryItems.FirstOrDefault(i => i.HasContext);

        if (contextItem != null)
        {
            ClearExistingContext();
        }
        else
        {
            Mediator.Send(new CloseGameInventoryRequest());
        }

        return Unit.Task;
    }
}