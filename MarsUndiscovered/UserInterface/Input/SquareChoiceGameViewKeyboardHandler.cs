using InputHandlers.Keyboard;

using MarsUndiscovered.Messages;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class SquareChoiceGameViewKeyboardHandler : BaseInventoryViewKeyboardHandler
    {
        public override void HandleKeyboardKeyDown(Keys[] keysDown, Keys keyInFocus, KeyboardModifier keyboardModifier)
        {
            if (ActionMap.ActionIs<CloseSquareChoiceRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new CloseSquareChoiceRequest());
        }
    }
}