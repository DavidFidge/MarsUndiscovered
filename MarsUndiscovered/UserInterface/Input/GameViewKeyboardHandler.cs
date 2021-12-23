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

            CheckGameKeys(keyInFocus, keyboardModifier);

            _cameraMovement.MoveCamera(keysDown);
        }

        private void CheckGameKeys(Keys keyInFocus, KeyboardModifier keyboardModifier)
        {
            if (ActionMap.ActionIs<MoveUpRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new MoveUpRequest());

            if (ActionMap.ActionIs<MoveUpLeftRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new MoveUpLeftRequest());

            if (ActionMap.ActionIs<MoveUpRightRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new MoveUpRightRequest());

            if (ActionMap.ActionIs<MoveDownRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new MoveDownRequest());

            if (ActionMap.ActionIs<MoveDownLeftRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new MoveDownLeftRequest());

            if (ActionMap.ActionIs<MoveDownRightRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new MoveDownRightRequest());

            if (ActionMap.ActionIs<MoveLeftRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new MoveLeftRequest());

            if (ActionMap.ActionIs<MoveRightRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new MoveRightRequest());
        }

        public override void HandleKeyboardKeyLost(Keys[] keysDown, KeyboardModifier keyboardModifier)
        {
            _cameraMovement.MoveCamera(keysDown);
        }

        public override void HandleKeyboardKeysReleased()
        {
            Mediator.Send(new MoveViewRequest(CameraMovementType.None));
        }

        public override void HandleKeyboardKeyRepeat(Keys repeatingKey, KeyboardModifier keyboardModifier)
        {
            CheckGameKeys(repeatingKey, keyboardModifier);
        }
    }
}