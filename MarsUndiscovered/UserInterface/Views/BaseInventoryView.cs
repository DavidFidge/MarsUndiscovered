using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.MonoGame.Core.View.Extensions;

using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

using GeonBit.UI.Entities;

using MarsUndiscovered.Components;

using Microsoft.Xna.Framework;

namespace MarsUndiscovered.UserInterface.Views
{
    public abstract class BaseInventoryView<TViewModel, TData> : BaseMarsUndiscoveredView<TViewModel, TData>
        where TViewModel : BaseInventoryViewModel<TData>
        where TData : BaseInventoryData, new()
    {
        protected Panel InventoryPanel;
        protected Panel InventoryHoverPanel;
        protected Panel InventoryContainerPanel;
        protected SelectList InventoryItems;
        protected Label _inventoryLabel;

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
                .Skin(PanelSkin.Simple)
                .AutoHeight()
                .Hidden();

            InventoryContainerPanel.AddChild(InventoryHoverPanel);

            InventoryPanel = new Panel()
                .Anchor(Anchor.TopRight)
                .Width(0.45f)
                .Skin(PanelSkin.Simple)
                .AutoHeight();

            InventoryContainerPanel.AddChild(InventoryPanel);

            _inventoryLabel = new Label("Your Inventory:")
                .NoPadding()
                .Anchor(Anchor.Auto);

            InventoryPanel.AddChild(_inventoryLabel);

            InventoryItems = new SelectList()
                .NoSkin()
                .Anchor(Anchor.Auto)
                .AutoHeight()
                .NoPadding();

            InventoryItems.ExtraSpaceBetweenLines = -14;
            InventoryItems.LockSelection = true;
            InventoryPanel.AddChild(InventoryItems);

            RootPanel.AddChild(InventoryContainerPanel);
        }

        public override void Show()
        {
            InventoryItems.ClearItems();

            base.Show();

            var inventoryItems = _viewModel.GetInventoryItems();

            if (inventoryItems.IsEmpty())
                InventoryItems.AddItem($"Your pack is empty");

            foreach (var item in inventoryItems)
            {
                InventoryItems.AddItem(GetInventoryItemText(item));
            }
        }

        protected virtual string GetInventoryItemText(InventoryItem item)
        {
            return $"{item.KeyDescription} {item.ItemDescription}";
        }
    }
}