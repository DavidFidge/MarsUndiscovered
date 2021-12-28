using FrigidRogue.MonoGame.Core.UserInterface;

using InputHandlers.Keyboard;

using MarsUndiscovered.Messages;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class LoadGameViewKeyboardHandler : BaseKeyboardHandler
    {
        public override void HandleKeyboardKeyDown(Keys[] keysDown, Keys keyInFocus, KeyboardModifier keyboardModifier)
        {
            if (ActionMap.ActionIs<CloseLoadGameViewRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new CloseLoadGameViewRequest());
        }
    }
}