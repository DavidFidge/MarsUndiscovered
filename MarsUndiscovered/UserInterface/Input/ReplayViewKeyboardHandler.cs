using System;

using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Input.CameraMovementSpace;

using FrigidRogue.MonoGame.Core.Graphics.Camera;
using FrigidRogue.MonoGame.Core.Messages;
using FrigidRogue.MonoGame.Core.UserInterface;

using InputHandlers.Keyboard;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class ReplayViewKeyboardHandler : BaseKeyboardHandler
    {
        private readonly ICameraMovement _cameraMovement;

        public ReplayViewKeyboardHandler(ICameraMovement cameraMovement)
        {
            _cameraMovement = cameraMovement;
        }

        public override void HandleKeyboardKeyDown(Keys[] keysDown, Keys keyInFocus, KeyboardModifier keyboardModifier)
        {
            if (ActionMap.ActionIs<OpenInReplayOptionsRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new OpenInReplayOptionsRequest());

            if (ActionMap.ActionIs<NextReplayCommandRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new NextReplayCommandRequest());

            if (keyInFocus == Keys.F12)
                Environment.Exit(0);

            _cameraMovement.MoveCamera(keysDown);
        }

        public override void HandleKeyboardKeyLost(Keys[] keysDown, KeyboardModifier keyboardModifier)
        {
            _cameraMovement.MoveCamera(keysDown);
        }

        public override void HandleKeyboardKeysReleased()
        {
            Mediator.Send(new MoveViewRequest(CameraMovementType.None));
        }
    }
}