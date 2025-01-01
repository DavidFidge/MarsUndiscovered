using System.Threading;
using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.MonoGame.Core.View.Extensions;
using GeonBit.UI.Entities;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Views;

public abstract class BaseInventoryView<TViewModel, TData> : BaseMarsUndiscoveredView<TViewModel, TData>, IInventoryView,
    IRequestHandler<CloseGameInventoryContextRequest>,
    IRequestHandler<InventoryItemSelectionCycleRequest>
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
            .Anchor(Anchor.Center)
            .SkinNone()
            .Size(0.8f, 0.8f)
            .Offset(0.1f, 0.1f)
            .NoPadding();

        InventoryItemDescriptionPanel = new Panel()
            .Anchor(Anchor.TopRight)
            .Offset(new Vector2(0.5f, 0f))
            .Width(0.5f)
            .Skin(PanelSkin.Alternative)
            .AutoHeight();
        
        InventoryItemDescriptionPanelText = new RichParagraph()
            .Anchor(Anchor.Auto);

        InventoryItemDescriptionPanel.AddChild(InventoryItemDescriptionPanelText);

        InventoryContainerPanel.AddChild(InventoryItemDescriptionPanel);
        
        InventoryPanel = new Panel()
            .Anchor(Anchor.TopLeft)
            .Width(0.5f)
            .Skin(PanelSkin.Alternative)
            .HeightOfParent();

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
            InventoryItems[i].SetInventoryItem(inventoryItems[i], InventoryMode);
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
                item.OnInventoryPanelEnter();
        }
    }

    protected virtual void PerformFocusKeyAction(InventoryItem focusItem, Keys requestKey)
    {
    }

    public virtual void ClearFocussedItem(InventoryItem inventoryItem)
    {
        InventoryItemDescriptionPanel.Hidden();
    }

    public virtual void AfterItemFocussed(InventoryItem inventoryItem)
    {
        if (inventoryItem != null)
        {
            foreach (var item in InventoryItems)
            {
                item.HideKeyDescription();
            }
            
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
            inventoryItemPanel.ShowKeyDescription();
        }
    }

    public void OnMouseDown(InventoryItem inventoryItem)
    {
        PerformInventoryModeAction(inventoryItem);
    }

    protected virtual void PerformInventoryModeAction(InventoryItem focusItem)
    {
    }

    public void Handle(CloseGameInventoryContextRequest request)
    {
        if (!IsVisible)
            return;
        
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

    public void Handle(InventoryItemSelectionCycleRequest request)
    {
        if (!InventoryItems.Any())
            return;
        
        var focusItem = InventoryItems.FirstOrDefault(i => i.HasFocus);
        var newFocusItem = focusItem;
        
        // Inventory items always holds 26 slots for each key which become visible and invisible
        if (focusItem != null)
        {
            var index = InventoryItems.IndexOf(focusItem);

            if (request.InventoryItemSelectionCycleRequestType == InventoryItemSelectionCycleRequestType.Next)
            {
                for (var i = index + 1; i < InventoryItems.Count; i++)
                {
                    if (InventoryItems[i].IsVisible())
                    {
                        focusItem = InventoryItems[i];
                        break;
                    }
                }

                if (focusItem == newFocusItem)
                {
                    for (var i = 0; i < index; i++)
                    {
                        if (InventoryItems[i].IsVisible())
                        {
                            focusItem = InventoryItems[i];
                            break;
                        }
                    }
                }
            }
            else
            {
                for (var i = index - 1; i >= 0; i--)
                {
                    if (InventoryItems[i].IsVisible())
                    {
                        focusItem = InventoryItems[i];
                        break;
                    }
                }

                if (focusItem == newFocusItem)
                {
                    for (var i = InventoryItems.Count - 1; i > index; i--)
                    {
                        if (InventoryItems[i].IsVisible())
                        {
                            focusItem = InventoryItems[i];
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            if (request.InventoryItemSelectionCycleRequestType == InventoryItemSelectionCycleRequestType.Next)
            {
                for (var i = 0; i < InventoryItems.Count; i++)
                {
                    if (InventoryItems[i].IsVisible())
                    {
                        focusItem = InventoryItems[i];
                        break;
                    }
                }
            }
            else
            {
                for (var i = InventoryItems.Count - 1; i >= 0; i--)
                {
                    if (InventoryItems[i].IsVisible())
                    {
                        focusItem = InventoryItems[i];
                        break;
                    }
                }
            }
        }
 
        if (focusItem != null)
            focusItem.OnInventoryPanelEnter();
    }
}