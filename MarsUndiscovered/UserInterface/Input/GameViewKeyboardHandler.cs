using MarsUndiscovered.Messages;
using InputHandlers.Keyboard;

using MarsUndiscovered.UserInterface.Views;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class GameViewKeyboardHandler : BaseGameViewKeyboardHandler
    {
        private readonly Options _options;
        private Keys? _ctrlArrowKey = null;

        public GameViewKeyboardHandler(Options options) : base()
        {
            _options = options;
        }

        public override void HandleKeyboardKeyDown(Keys[] keysDown, Keys keyInFocus, KeyboardModifier keyboardModifier)
        {
            base.HandleKeyboardKeyDown(keysDown, keyInFocus, keyboardModifier);

            if (ActionMap.ActionIs<OpenInGameOptionsRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new OpenInGameOptionsRequest());

            if (ActionMap.ActionIs<HotBarItemRequest>(keyInFocus))
            {
                Mediator.Send(new HotBarItemRequest(keyInFocus));
            }
            
            if (ActionMap.ActionIs<OpenGameInventoryRequest>(keyInFocus, keyboardModifier))
            {
                var actionName = ActionMap.ActionName<OpenGameInventoryRequest>(keyInFocus, keyboardModifier);

                var inventoryMode = InventoryMode.View;

                switch (actionName)
                {
                    case OpenGameInventoryRequest.Equip:
                        inventoryMode = InventoryMode.Equip;
                        break;
                    case OpenGameInventoryRequest.Unequip:
                        inventoryMode = InventoryMode.Unequip;
                        break;
                    case OpenGameInventoryRequest.Drop:
                        inventoryMode = InventoryMode.Drop;
                        break;
                    case OpenGameInventoryRequest.Apply:
                        inventoryMode = InventoryMode.Apply;
                        break;
                }

                Mediator.Send(new OpenGameInventoryRequest(inventoryMode));
            }

            if (ActionMap.ActionIs<AutoExploreRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new AutoExploreRequest());

            if (_options.WizardMode)
            {
                if (ActionMap.ActionIs<WizardModeNextLevelRequest>(keyInFocus, keyboardModifier))
                    Mediator.Send(new WizardModeNextLevelRequest());
                
                if (ActionMap.ActionIs<WizardModePreviousLevelRequest>(keyInFocus, keyboardModifier))
                    Mediator.Send(new WizardModePreviousLevelRequest());
                
                if (ActionMap.ActionIs<ToggleFpsRequest>(keyInFocus, keyboardModifier))
                    Mediator.Send(new ToggleFpsRequest());
                
                if (ActionMap.ActionIs<OpenConsoleRequest>(keyInFocus, keyboardModifier))
                    Mediator.Send(new OpenConsoleRequest());
            }
            
            ProcessMovement(keyInFocus, keyboardModifier);
            
            if (ActionMap.ActionIs<RangeAttackEquippedWeaponRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new RangeAttackEquippedWeaponRequest());
        }

        public override void HandleKeyboardKeyRepeat(Keys repeatingKey, KeyboardModifier keyboardModifier)
        {
            base.HandleKeyboardKeyRepeat(repeatingKey, keyboardModifier);

            ProcessMovement(repeatingKey, keyboardModifier);
        }

        private void ProcessMovement(Keys keyInFocus, KeyboardModifier keyboardModifier)
        {
            if ((keyboardModifier & KeyboardModifier.Ctrl) == 0)
            {
                _ctrlArrowKey = null;

                if (ActionMap.ActionIs<MoveUpRequest>(keyInFocus, keyboardModifier))
                    Mediator.Send(new MoveUpRequest());

                if (ActionMap.ActionIs<MoveUpLeftRequest>(keyInFocus, keyboardModifier))
                    Mediator.Send(new MoveUpLeftRequest());

                if (ActionMap.ActionIs<MoveUpRightRequest>(keyInFocus, keyboardModifier))
                    Mediator.Send(new MoveUpRightRequest());

                if (ActionMap.ActionIs<MoveDownRequest>(keyInFocus, keyboardModifier))
                    Mediator.Send(new MoveDownRequest());

                if (ActionMap.ActionIs<MoveDownLeftRequest>(keyInFocus, keyboardModifier))
                    Mediator.Send(new MoveDownLeftRequest());

                if (ActionMap.ActionIs<MoveDownRightRequest>(keyInFocus, keyboardModifier))
                    Mediator.Send(new MoveDownRightRequest());

                if (ActionMap.ActionIs<MoveLeftRequest>(keyInFocus, keyboardModifier))
                    Mediator.Send(new MoveLeftRequest());

                if (ActionMap.ActionIs<MoveRightRequest>(keyInFocus, keyboardModifier))
                    Mediator.Send(new MoveRightRequest());

                if (ActionMap.ActionIs<MoveWaitRequest>(keyInFocus, keyboardModifier))
                    Mediator.Send(new MoveWaitRequest());
            }
            else
            {
                if (_ctrlArrowKey != null)
                {
                    if (keyInFocus != _ctrlArrowKey)
                    {
                        var keyUp = _ctrlArrowKey == Keys.Up || keyInFocus == Keys.Up;
                        var keyDown = _ctrlArrowKey == Keys.Down || keyInFocus == Keys.Down;
                        var keyLeft = _ctrlArrowKey == Keys.Left || keyInFocus == Keys.Left;
                        var keyRight = _ctrlArrowKey == Keys.Right || keyInFocus == Keys.Right;

                        if (keyUp && keyLeft)
                        {
                            Mediator.Send(new MoveUpLeftRequest());
                            _ctrlArrowKey = null;
                        }
                        else if (keyDown && keyRight)
                        {
                            Mediator.Send(new MoveDownRightRequest());
                            _ctrlArrowKey = null;
                        }
                        else if (keyDown && keyLeft)
                        {
                            Mediator.Send(new MoveDownLeftRequest());
                            _ctrlArrowKey = null;
                        }
                        else if (keyUp && keyRight)
                        {
                            Mediator.Send(new MoveUpRightRequest());
                            _ctrlArrowKey = null;
                        }
                        else
                        {
                            _ctrlArrowKey = null;
                        }
                    }
                }
                else
                {
                    _ctrlArrowKey = keyInFocus;
                }
            }
        }
    }
}