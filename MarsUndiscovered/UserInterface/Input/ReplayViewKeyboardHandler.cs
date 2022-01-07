using System;

using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Input.CameraMovementSpace;

using InputHandlers.Keyboard;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class ReplayViewKeyboardHandler : BaseGameViewKeyboardHandler
    {
        private readonly ICameraMovement _cameraMovement;

        public ReplayViewKeyboardHandler(ICameraMovement cameraMovement) : base(cameraMovement)
        {
            _cameraMovement = cameraMovement;
        }

        public override void HandleKeyboardKeyDown(Keys[] keysDown, Keys keyInFocus, KeyboardModifier keyboardModifier)
        {
            base.HandleKeyboardKeyDown(keysDown, keyInFocus, keyboardModifier);

            if (ActionMap.ActionIs<OpenInReplayOptionsRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new OpenInReplayOptionsRequest());

            if (ActionMap.ActionIs<NextReplayCommandRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new NextReplayCommandRequest());
        }
    }
}