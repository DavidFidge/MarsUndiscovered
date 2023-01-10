using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Input.CameraMovementSpace;

using InputHandlers.Keyboard;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class ReplayViewKeyboardHandler : BaseGameViewKeyboardHandler
    {
        public override void HandleKeyboardKeyDown(Keys[] keysDown, Keys keyInFocus, KeyboardModifier keyboardModifier)
        {
            base.HandleKeyboardKeyDown(keysDown, keyInFocus, keyboardModifier);

            if (ActionMap.ActionIs<OpenInReplayOptionsRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new OpenInReplayOptionsRequest());

            if (ActionMap.ActionIs<NextReplayCommandRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new NextReplayCommandRequest());

            if (ActionMap.ActionIs<OpenReplayInventoryRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new OpenReplayInventoryRequest());
        }

        public override void HandleKeyboardKeyRepeat(Keys repeatingKey, KeyboardModifier keyboardModifier)
        {
            base.HandleKeyboardKeyRepeat(repeatingKey, keyboardModifier);

            if (ActionMap.ActionIs<NextReplayCommandRequest>(repeatingKey, keyboardModifier))
                Mediator.Send(new NextReplayCommandRequest());
        }
    }
}