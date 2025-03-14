﻿using System.Threading;
using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.Interfaces.UserInterface;
using FrigidRogue.MonoGame.Core.View.Extensions;
using GeonBit.UI.Entities;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Views
{
    public class InventoryGameView : BaseInventoryView<InventoryGameViewModel, InventoryGameData>,
        IRequestHandler<InventoryItemSelectionRequest>,
        IRequestHandler<LeftClickInventoryGameViewRequest>,
        IRequestHandler<AssignHotBarItemRequest>
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

        public override void AfterItemFocussed(InventoryItem inventoryItem)
        {
            base.AfterItemFocussed(inventoryItem);

            _equipButton.Enabled = inventoryItem.CanEquip;
            _unequipButton.Enabled = inventoryItem.CanUnequip;
            _dropButton.Enabled = inventoryItem.CanDrop;
            _applyButton.Enabled = inventoryItem.CanApply;

            if (InventoryMode == InventoryMode.ReadOnly ||
                InventoryMode == InventoryMode.Identify ||
                InventoryMode == InventoryMode.Enchant)
            {
                _equipButton.Enabled = false;
                _unequipButton.Enabled = false;
                _dropButton.Enabled = false;
                _applyButton.Enabled = false;
            }
            
            if (InventoryMode == InventoryMode.Apply)
            {
                _equipButton.Enabled = false;
                _unequipButton.Enabled = false;
                _dropButton.Enabled = false;
            }
            
            if (InventoryMode == InventoryMode.Drop)
            {
                _equipButton.Enabled = false;
                _unequipButton.Enabled = false;
                _applyButton.Enabled = false;
            }
            
            if (InventoryMode == InventoryMode.Equip)
            {
                _unequipButton.Enabled = false;
                _dropButton.Enabled = false;
                _applyButton.Enabled = false;
            }
            
            if (InventoryMode == InventoryMode.Unequip)
            {
                _equipButton.Enabled = false;
                _dropButton.Enabled = false;
                _applyButton.Enabled = false;
            }
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
                case InventoryMode.ReadOnly:
                case InventoryMode.View:
                    InventoryLabel.Text = "Your Inventory:";
                    break;
                case InventoryMode.Equip:
                    InventoryLabel.Text = "Equip what?";
                    break;
                case InventoryMode.Unequip:
                    InventoryLabel.Text = "Remove (unequip) what?";
                    break;
                case InventoryMode.Drop:
                    InventoryLabel.Text = "Drop what?";
                    break;
                case InventoryMode.Apply:
                    InventoryLabel.Text = "Apply (use) what?";
                    break;
                case InventoryMode.Enchant:
                    InventoryLabel.Text = "Enhance what?";
                    break;
                case InventoryMode.Identify:
                    InventoryLabel.Text = "Identify what?";
                    break;   
            }
        }

        public void Handle(InventoryItemSelectionRequest request)
        {
            if (!IsVisible)
                return;

            var focusItem = InventoryItems.FirstOrDefault(i => i.HasFocus);

            if (focusItem != null &&
                InventoryMode != InventoryMode.View &&
                InventoryMode != InventoryMode.ReadOnly)
            {
                if (request.Key == Keys.Enter)
                   PerformInventoryModeAction(focusItem.InventoryItem);
                else
                    PerformFocusKeyActionSilentlyFail(focusItem.InventoryItem, request.Key);
                
                return;
            }
            
            switch (InventoryMode)
            {
                case InventoryMode.View:
                    PerformKeyAction(request.Key);
                    break;
                case InventoryMode.Equip:
                    _viewModel.EquipRequest(request.Key);
                    break;
                case InventoryMode.Unequip:
                    _viewModel.UnequipRequest(request.Key);
                    break;
                case InventoryMode.Drop:
                    _viewModel.DropRequest(request.Key);
                    break;
                case InventoryMode.Apply:
                    _viewModel.ApplyRequest(request.Key);
                    break;
                case InventoryMode.Enchant:
                    _viewModel.EnchantItemRequest(request.Key);
                    break;
                case InventoryMode.Identify:
                    _viewModel.IdentifyItemRequest(request.Key);
                    break;
                case InventoryMode.ReadOnly:
                    PerformKeyAction(request.Key);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected void PerformFocusKeyActionSilentlyFail(InventoryItem focusItem, Keys requestKey)
        {
            if ((_actionMap.ActionIs<OpenGameInventoryRequest>(requestKey, OpenGameInventoryRequest.Equip) &&
                 focusItem.CanEquip) ||
                (_actionMap.ActionIs<OpenGameInventoryRequest>(requestKey, OpenGameInventoryRequest.Drop) &&
                 focusItem.CanDrop) ||
                (_actionMap.ActionIs<OpenGameInventoryRequest>(requestKey, OpenGameInventoryRequest.Unequip) &&
                 focusItem.CanUnequip) ||
                (_actionMap.ActionIs<OpenGameInventoryRequest>(requestKey, OpenGameInventoryRequest.Apply) &&
                 focusItem.CanApply))
            {
                PerformFocusKeyAction(focusItem, requestKey);
            }
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
                case InventoryMode.Equip:
                    _viewModel.EquipRequest(focusItem.Key);
                    break;
                case InventoryMode.Unequip:
                    _viewModel.UnequipRequest(focusItem.Key);
                    break;
                case InventoryMode.Drop:
                    _viewModel.DropRequest(focusItem.Key);
                    break;
                case InventoryMode.Apply:
                    _viewModel.ApplyRequest(focusItem.Key);
                    break;
                case InventoryMode.Enchant:
                    _viewModel.EnchantItemRequest(focusItem.Key);
                    break;
                case InventoryMode.Identify:
                    _viewModel.EnchantItemRequest(focusItem.Key);
                    break;
            }
        }

        public override void Hide()
        {
            ClearFocus();
            base.Hide();
        }

        public void Handle(LeftClickInventoryGameViewRequest request)
        {
            HideIfMouseOver();
        }

        protected override void ClosingInventoryNoAction()
        {
            _viewModel.ClosingInventoryNoAction(InventoryMode);
        }

        public void Handle(AssignHotBarItemRequest request)
        {
            var focusItem = InventoryItems.FirstOrDefault(i => i.HasFocus);

            if (focusItem != null && focusItem.InventoryItem.CanAssignHotKey)
            {
                _viewModel.AssignHotBarItem(focusItem.InventoryItem.Key, request.Key);
                Mediator.Send(new RefreshHotBarRequest(focusItem.InventoryItem.ItemId));
            }
        }
    }
}