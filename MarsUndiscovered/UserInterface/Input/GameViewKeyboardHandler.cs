﻿using System;

using Augmented.Messages;
using Augmented.UserInterface.Input.CameraMovementSpace;

using DavidFidge.MonoGame.Core.Graphics.Camera;
using DavidFidge.MonoGame.Core.UserInterface;

using InputHandlers.Keyboard;

using Microsoft.Xna.Framework.Input;

namespace Augmented.UserInterface.Input
{
    public class GameViewKeyboardHandler : BaseKeyboardHandler
    {
        private readonly ICameraMovement _cameraMovement;

        public GameViewKeyboardHandler(ICameraMovement cameraMovement)
        {
            _cameraMovement = cameraMovement;
        }
        public override void HandleKeyboardKeyDown(Keys[] keysDown, Keys keyInFocus, KeyboardModifier keyboardModifier)
        {
            if (ActionMap.ActionIs<OpenInGameOptionsRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new OpenInGameOptionsRequest());

            if (ActionMap.ActionIs<OpenConsoleRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new OpenConsoleRequest());

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
            Mediator.Send(new Move3DViewRequest(CameraMovement.None));
        }
    }
}