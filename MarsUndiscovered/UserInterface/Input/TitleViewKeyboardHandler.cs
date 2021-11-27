using DavidFidge.MonoGame.Core.Messages;
using DavidFidge.MonoGame.Core.UserInterface;

using InputHandlers.Keyboard;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class TitleViewKeyboardHandler : BaseKeyboardHandler
    {
        public override void HandleKeyboardKeyDown(Keys[] keysDown, Keys keyInFocus, KeyboardModifier keyboardModifier)
        {
            if (ActionMap.ActionIs<ExitGameRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new ExitGameRequest());
        }
    }
}