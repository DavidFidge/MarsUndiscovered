using System;

using MarsUndiscovered.UserInterface.Input.CameraMovementSpace;

using FrigidRogue.MonoGame.Core.Graphics.Camera;
using FrigidRogue.MonoGame.Core.Messages;
using FrigidRogue.MonoGame.Core.UserInterface;

using InputHandlers.Keyboard;

using MarsUndiscovered.Messages;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class BaseGameViewKeyboardHandler : BaseKeyboardHandler
    {
        protected readonly ICameraMovement _cameraMovement;

        public BaseGameViewKeyboardHandler(ICameraMovement cameraMovement)
        {
            _cameraMovement = cameraMovement;
        }

        public override void HandleKeyboardKeyDown(Keys[] keysDown, Keys keyInFocus, KeyboardModifier keyboardModifier)
        {
            if (keyInFocus == Keys.F12)
                Environment.Exit(0);

            if (keyInFocus == Keys.G)
                Mediator.Publish(new ToggleShowGoalMapNotification());

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