using FrigidRogue.MonoGame.Core.Messages;
using FrigidRogue.MonoGame.Core.UserInterface;

using InputHandlers.Keyboard;

using MarsUndiscovered.Messages;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class TitleViewKeyboardHandler : BaseKeyboardHandler
    {
        public override void HandleKeyboardKeyDown(Keys[] keysDown, Keys keyInFocus, KeyboardModifier keyboardModifier)
        {
            if (ActionMap.ActionIs<ExitGameRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new ExitGameRequest());

            if (ActionMap.ActionIs<NewGameRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new NewGameRequest());
        }
    }
}