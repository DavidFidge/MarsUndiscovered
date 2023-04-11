using MarsUndiscovered.Messages;

using FrigidRogue.MonoGame.Core.UserInterface;

using InputHandlers.Keyboard;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class GameOptionsKeyboardHandler : BaseKeyboardHandler
    {
        public override void HandleKeyboardKeyDown(Keys[] keysDown, Keys keyInFocus, KeyboardModifier keyboardModifier)
        {
            if (ActionMap.ActionIs<CloseGameOptionsRequest>(keyInFocus, keyboardModifier)
                || ActionMap.ActionIs<CloseInGameGameOptionsRequest>(keyInFocus, keyboardModifier))
            {
                Mediator.Send(new CloseGameOptionsRequest());
                Mediator.Send(new CloseInGameGameOptionsRequest());
            }
        }
    }
}