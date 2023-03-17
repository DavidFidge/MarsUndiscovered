using InputHandlers.Keyboard;

using MarsUndiscovered.Messages;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class InventoryGameViewKeyboardHandler : BaseInventoryViewKeyboardHandler
    {
        public override void HandleKeyboardKeyDown(Keys[] keysDown, Keys keyInFocus, KeyboardModifier keyboardModifier)
        {
            if (ActionMap.ActionIs<CloseGameInventoryContextRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new CloseGameInventoryContextRequest());

            if (ActionMap.ActionIs<InventoryItemSelectionRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new InventoryItemSelectionRequest(keyInFocus));
        }
    }
}