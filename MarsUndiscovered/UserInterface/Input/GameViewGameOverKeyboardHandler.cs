﻿using System;

using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Input.CameraMovementSpace;

using InputHandlers.Keyboard;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class GameViewGameOverKeyboardHandler : BaseGameViewKeyboardHandler
    {
        public GameViewGameOverKeyboardHandler(ICameraMovement cameraMovement) : base(cameraMovement)
        {
        }

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