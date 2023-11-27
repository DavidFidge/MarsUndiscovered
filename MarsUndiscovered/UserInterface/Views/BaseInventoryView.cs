using System.Threading;
using System.Threading.Tasks;
using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.MonoGame.Core.View.Extensions;

using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

using GeonBit.UI.Entities;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Messages;
using MediatR;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Views;

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
            .SkinNone()
            .NoPadding()
            .Height(0.79f)
            .Offset(new Vector2(20f, 120f));

        InventoryItemDescriptionPanel = new Panel()
            .Anchor(Anchor.TopLeft)
            .Width(0.5f)
            .Skin(PanelSkin.Alternative)
            .AutoHeight()
            .Hidden();

        InventoryItemDescriptionPanelText = new RichParagraph()
            .Anchor(Anchor.Auto);

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
            var inventoryItemPanel = new InventoryItemPanel(this, Assets);
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
        var focusItem = InventoryItems.FirstOrDefault(i => i.HasFocus);

        if (focusItem != null)
        {
            PerformFocusKeyAction(focusItem.InventoryItem, requestKey);
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

    protected virtual void PerformFocusKeyAction(InventoryItem focusItem, Keys requestKey)
    {
    }

    public virtual void ClearFocussedItem(InventoryItem inventoryItem)
    {
        InventoryItemDescriptionPanel.Hidden();
    }

    public virtual void SetFocussedItem(InventoryItem inventoryItem)
    {
        if (inventoryItem != null)
        {
            InventoryItemDescriptionPanelText.Text =
                $"{inventoryItem.ItemDescription}\n\n{inventoryItem.LongDescription}";
            InventoryItemDescriptionPanel.Visible();
            InventoryItemDescriptionPanel.ForceDirty();
        }
    }

    public void ClearFocus()
    {
        foreach (var inventoryItemPanel in InventoryItems)
        {
            inventoryItemPanel.OnInventoryPanelLeave();
        }
    }

    public void OnMouseDown(InventoryItem inventoryItem)
    {
        PerformInventoryModeAction(inventoryItem);
    }

    protected virtual void PerformInventoryModeAction(InventoryItem focusItem)
    {
    }

    public Task<Unit> Handle(CloseGameInventoryContextRequest request, CancellationToken cancellationToken)
    {
        if (!IsVisible)
            return Unit.Task;
        
        var focusItem = InventoryItems.FirstOrDefault(i => i.HasFocus);

        if (focusItem != null)
        {
            ClearFocus();
        }
        else
        {
            ClosingInventoryNoAction();
            Mediator.Send(new CloseGameInventoryRequest());
        }

        return Unit.Task;
    }

    protected virtual void ClosingInventoryNoAction()
    {
    }
    
    public void HideIfMouseOver()
    {
        if (!InventoryPanel.IsMouseOver && !InventoryItemDescriptionPanel.IsMouseOver)
        {
            Hide();
        }
    }
}