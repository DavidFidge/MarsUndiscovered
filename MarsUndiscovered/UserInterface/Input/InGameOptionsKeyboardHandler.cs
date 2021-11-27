using MarsUndiscovered.Messages;

using DavidFidge.MonoGame.Core.UserInterface;

using InputHandlers.Keyboard;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class InGameOptionsKeyboardHandler : BaseKeyboardHandler
    {
        public override void HandleKeyboardKeyDown(Keys[] keysDown, Keys keyInFocus, KeyboardModifier keyboardModifier)
        {
            if (ActionMap.ActionIs<CloseInGameOptionsRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new CloseInGameOptionsRequest());
        }
    }
}