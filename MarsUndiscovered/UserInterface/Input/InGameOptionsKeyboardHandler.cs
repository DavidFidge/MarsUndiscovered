using FrigidRogue.MonoGame.Core.UserInterface;
using InputHandlers.Keyboard;
using MarsUndiscovered.Messages;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class InGameOptionsKeyboardHandler : BaseKeyboardHandler
    {
        public override void HandleKeyboardKeyDown(Keys[] keysDown, Keys keyInFocus, KeyboardModifier keyboardModifier)
        {
            if (ActionMap.ActionIs<CloseInGameOptionsRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new CloseInGameOptionsRequest());

            if (ActionMap.ActionIs<QuitToTitleRequest>(keyInFocus, keyboardModifier))
            {
                Mediator.Send(new CloseInGameOptionsRequest());
                Mediator.Send(new QuitToTitleRequest());
            }

            if (ActionMap.ActionIs<OpenSaveGameViewRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new OpenSaveGameViewRequest());
        }
    }
}