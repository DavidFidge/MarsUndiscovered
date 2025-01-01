using InputHandlers.Keyboard;
using MarsUndiscovered.Messages;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class GameViewGameOverKeyboardHandler : BaseGameViewKeyboardHandler
    {
        public override void HandleKeyboardKeyDown(Keys[] keysDown, Keys keyInFocus, KeyboardModifier keyboardModifier)
        {
            base.HandleKeyboardKeyDown(keysDown, keyInFocus, keyboardModifier);

            if (ActionMap.ActionIs<OpenInGameOptionsRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new OpenInGameOptionsRequest());

            if (keyInFocus == Keys.Space || keyInFocus == Keys.Enter)
                Mediator.Send(new QuitToTitleRequest());
        }
    }
}