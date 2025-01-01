using FrigidRogue.MonoGame.Core.UserInterface;
using InputHandlers.Keyboard;
using MarsUndiscovered.Messages;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class DeveloperToolsKeyboardHandler : BaseKeyboardHandler
    {
        public override void HandleKeyboardKeyDown(Keys[] keysDown, Keys keyInFocus, KeyboardModifier keyboardModifier)
        {
            if (ActionMap.ActionIs<CloseDeveloperToolsViewRequest>(keyInFocus, keyboardModifier))
            {
                Mediator.Send(new CloseDeveloperToolsViewRequest());
            }
        }
    }
}