using InputHandlers.Keyboard;
using MarsUndiscovered.Messages;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class InventoryReplayViewKeyboardHandler : BaseInventoryViewKeyboardHandler
    {
        public override void HandleKeyboardKeyDown(Keys[] keysDown, Keys keyInFocus, KeyboardModifier keyboardModifier)
        {
            if (ActionMap.ActionIs<CloseReplayInventoryRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new CloseReplayInventoryRequest());
        }
    }
}