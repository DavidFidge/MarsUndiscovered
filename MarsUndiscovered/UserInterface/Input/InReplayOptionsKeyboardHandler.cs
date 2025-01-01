using FrigidRogue.MonoGame.Core.UserInterface;
using InputHandlers.Keyboard;
using MarsUndiscovered.Messages;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class InReplayOptionsKeyboardHandler : BaseKeyboardHandler
    {
        public override void HandleKeyboardKeyDown(Keys[] keysDown, Keys keyInFocus, KeyboardModifier keyboardModifier)
        {
            if (ActionMap.ActionIs<CloseInReplayOptionsRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new CloseInReplayOptionsRequest());

            if (ActionMap.ActionIs<QuitToTitleRequest>(keyInFocus, keyboardModifier))
            {
                Mediator.Send(new CloseInReplayOptionsRequest());
                Mediator.Send(new QuitToTitleRequest());
            }
        }
    }
}