using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Input.CameraMovementSpace;

using InputHandlers.Keyboard;

using MarsUndiscovered.UserInterface.Views;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class GameViewKeyboardHandler : BaseGameViewKeyboardHandler
    {
        public GameViewKeyboardHandler(ICameraMovement cameraMovement) : base(cameraMovement)
        {
        }

        public override void HandleKeyboardKeyDown(Keys[] keysDown, Keys keyInFocus, KeyboardModifier keyboardModifier)
        {
            base.HandleKeyboardKeyDown(keysDown, keyInFocus, keyboardModifier);

            if (ActionMap.ActionIs<OpenInGameOptionsRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new OpenInGameOptionsRequest());

            if (ActionMap.ActionIs<OpenConsoleRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new OpenConsoleRequest());

            if (ActionMap.ActionIs<OpenGameInventoryRequest>(keyInFocus, keyboardModifier))
            {
                var actionName = ActionMap.ActionName<OpenGameInventoryRequest>(keyInFocus, keyboardModifier);

                var inventoryMode = InventoryMode.View;

                switch (actionName)
                {
                    case "Equip":
                        inventoryMode = InventoryMode.Equip;
                        break;
                    case "Unequip":
                        inventoryMode = InventoryMode.Unequip;
                        break;
                    case "Drop":
                        inventoryMode = InventoryMode.Drop;
                        break;
                }

                Mediator.Send(new OpenGameInventoryRequest(inventoryMode));
            }

            ProcessMovement(keyInFocus, keyboardModifier);
        }

        public override void HandleKeyboardKeyRepeat(Keys repeatingKey, KeyboardModifier keyboardModifier)
        {
            base.HandleKeyboardKeyRepeat(repeatingKey, keyboardModifier);

            ProcessMovement(repeatingKey, keyboardModifier);
        }

        private void ProcessMovement(Keys keyInFocus, KeyboardModifier keyboardModifier)
        {
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
    }
}