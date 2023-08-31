using System.Threading;
using System.Threading.Tasks;
using FrigidRogue.MonoGame.Core.Interfaces.UserInterface;
using GeonBit.UI.Entities;
using FrigidRogue.MonoGame.Core.View.Extensions;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

using MediatR;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Views
{
    public class InventoryGameView : BaseInventoryView<InventoryGameViewModel, InventoryGameData>,
        IRequestHandler<InventoryItemSelectionRequest>
    {
        private readonly IActionMap _actionMap;
        private Button _equipButton;
        private Button _unequipButton;
        private Button _dropButton;
        private Button _applyButton;
        protected Panel InventoryItemButtonPanel { get; set; }

        public InventoryGameView(
            InventoryGameViewModel inventoryGameViewModel,
            IActionMap actionMap
        ) : base(inventoryGameViewModel)
        {
            _actionMap = actionMap;
        }

        protected override void InitializeInternal()
        {
            base.InitializeInternal();
            
            InventoryItemButtonPanel = new Panel()
                .Anchor(Anchor.Auto)
                .SkinNone()
                .AutoHeight()
                .WidthOfContainer()
                .NoPadding();
            
            _equipButton = new Button("{{YELLOW}}e{{DEFAULT}}quip")
                .Anchor(Anchor.AutoInline)
                .WidthTextWithPadding(50)
                .SkinAlternative();
        
            _equipButton.OnClick += OnEquip;

            InventoryItemButtonPanel.AddChild(_equipButton);
        
            _unequipButton = new Button("{{YELLOW}}r{{DEFAULT}}emove")
                .Anchor(Anchor.AutoInline)
                .WidthTextWithPadding(50)
                .Offset(50, 0)
                .SkinAlternative();
        
            _unequipButton.OnClick += OnUnequip; 
            
            InventoryItemButtonPanel.AddChild(_unequipButton);

            _dropButton = new Button("{{YELLOW}}d{{DEFAULT}}rop")
                .Anchor(Anchor.AutoInline)
                .WidthTextWithPadding(50)
                .Offset(50, 0)
                .SkinAlternative();
        
            _dropButton.OnClick += OnDrop;    
            
            InventoryItemButtonPanel.AddChild(_dropButton);
            
            _applyButton = new Button("{{YELLOW}}a{{DEFAULT}}pply")
                .Anchor(Anchor.AutoInline)
                .WidthTextWithPadding(50)
                .Offset(50, 0)
                .SkinAlternative();
        
            _applyButton.OnClick += OnApply;    
            
            InventoryItemButtonPanel.AddChild(_applyButton);

            InventoryItemDescriptionPanel.AddChild(InventoryItemButtonPanel);
        }

        public override void SetFocussedItem(InventoryItem inventoryItem)
        {
            base.SetFocussedItem(inventoryItem);

            _equipButton.Enabled = inventoryItem.CanEquip;
            _unequipButton.Enabled = inventoryItem.CanUnequip;
            _dropButton.Enabled = inventoryItem.CanDrop;
            _applyButton.Enabled = inventoryItem.CanApply;
        }

        private void OnEquip(Entity entity)
        {
            var focusItem = InventoryItems.FirstOrDefault(i => i.HasFocus);

            if (focusItem != null && focusItem.InventoryItem.CanEquip)
            {
                _viewModel.EquipRequest(focusItem.InventoryItem.Key);
            }
        }
    
        private void OnUnequip(Entity entity)
        {
            var focusItem = InventoryItems.FirstOrDefault(i => i.HasFocus);

            if (focusItem != null && focusItem.InventoryItem.CanUnequip)
            {
                _viewModel.UnequipRequest(focusItem.InventoryItem.Key);
            }
        }
    
        private void OnDrop(Entity entity)
        {
            var focusItem = InventoryItems.FirstOrDefault(i => i.HasFocus);

            if (focusItem != null && focusItem.InventoryItem.CanDrop)
            {
                _viewModel.DropRequest(focusItem.InventoryItem.Key);
            }
        }
        
        private void OnApply(Entity entity)
        {
            var focusItem = InventoryItems.FirstOrDefault(i => i.HasFocus);

            if (focusItem != null && focusItem.InventoryItem.CanApply)
            {
                _viewModel.ApplyRequest(focusItem.InventoryItem.Key);
            }
        }

        public void SetInventoryMode(InventoryMode inventoryMode)
        {
            InventoryMode = inventoryMode;

            switch (inventoryMode)
            {
                case Views.InventoryMode.ReadOnly:
                case Views.InventoryMode.View:
                    InventoryLabel.Text = "Your Inventory:";
                    break;
                case Views.InventoryMode.Equip:
                    InventoryLabel.Text = "Equip what?";
                    break;
                case Views.InventoryMode.Unequip:
                    InventoryLabel.Text = "Remove (unequip) what?";
                    break;
                case Views.InventoryMode.Drop:
                    InventoryLabel.Text = "Drop what?";
                    break;
                case Views.InventoryMode.Apply:
                    InventoryLabel.Text = "Apply (use) what?";
                    break;
            }
        }

        public Task<Unit> Handle(InventoryItemSelectionRequest request, CancellationToken cancellationToken)
        {
            if (!IsVisible)
                return Unit.Task;
            
            switch (InventoryMode)
            {
                case Views.InventoryMode.View:
                    base.PerformKeyAction(request.Key);
                    break;
                case Views.InventoryMode.Equip:
                    _viewModel.EquipRequest(request.Key);
                    break;
                case Views.InventoryMode.Unequip:
                    _viewModel.UnequipRequest(request.Key);
                    break;
                case Views.InventoryMode.Drop:
                    _viewModel.DropRequest(request.Key);
                    break;
                case Views.InventoryMode.Apply:
                    _viewModel.ApplyRequest(request.Key);
                    break;
                case Views.InventoryMode.ReadOnly:
                    base.PerformKeyAction(request.Key);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return Unit.Task;
        }

        protected override void PerformFocusKeyAction(InventoryItem focusItem, Keys requestKey)
        {
            if (_actionMap.ActionIs<OpenGameInventoryRequest>(requestKey, OpenGameInventoryRequest.Equip))
                _viewModel.EquipRequest(focusItem.Key);
            else if (_actionMap.ActionIs<OpenGameInventoryRequest>(requestKey, OpenGameInventoryRequest.Drop))
                _viewModel.DropRequest(focusItem.Key);
            else if (_actionMap.ActionIs<OpenGameInventoryRequest>(requestKey, OpenGameInventoryRequest.Unequip))
                _viewModel.UnequipRequest(focusItem.Key);
            else if (_actionMap.ActionIs<OpenGameInventoryRequest>(requestKey, OpenGameInventoryRequest.Apply))
                _viewModel.ApplyRequest(focusItem.Key);
        }
        
        protected override void PerformInventoryModeAction(InventoryItem focusItem)
        {
            switch (InventoryMode)
            {
                case Views.InventoryMode.Equip:
                    _viewModel.EquipRequest(focusItem.Key);
                    break;
                case Views.InventoryMode.Unequip:
                    _viewModel.UnequipRequest(focusItem.Key);
                    break;
                case Views.InventoryMode.Drop:
                    _viewModel.DropRequest(focusItem.Key);
                    break;
                case Views.InventoryMode.Apply:
                    _viewModel.ApplyRequest(focusItem.Key);
                    break;
                case Views.InventoryMode.Enchant:
                    _viewModel.EnchantItemRequest(focusItem.Key);
                    break;
            }
        }

        public override void Hide()
        {
            ClearFocus();
            base.Hide();
        }
    }
}