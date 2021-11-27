using MarsUndiscovered.Messages;

using DavidFidge.MonoGame.Core.UserInterface;

using InputHandlers.Keyboard;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class VideoOptionsKeyboardHandler : BaseKeyboardHandler
    {
        public override void HandleKeyboardKeyDown(Keys[] keysDown, Keys keyInFocus, KeyboardModifier keyboardModifier)
        {
            if (ActionMap.ActionIs<CloseVideoOptionsRequest>(keyInFocus, keyboardModifier))
            {
                Mediator.Send(new SaveVideoOptionsRequest());
                Mediator.Send(new CloseOptionsViewRequest());
            }
        }
    }
}