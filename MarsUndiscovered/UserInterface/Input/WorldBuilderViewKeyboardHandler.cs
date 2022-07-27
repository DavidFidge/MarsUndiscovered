using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Input.CameraMovementSpace;

using InputHandlers.Keyboard;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class WorldBuilderViewKeyboardHandler : BaseGameViewKeyboardHandler
    {
        public WorldBuilderViewKeyboardHandler(ICameraMovement cameraMovement) : base(cameraMovement)
        {
        }

        public override void HandleKeyboardKeyDown(Keys[] keysDown, Keys keyInFocus, KeyboardModifier keyboardModifier)
        {
            base.HandleKeyboardKeyDown(keysDown, keyInFocus, keyboardModifier);

            if (ActionMap.ActionIs<OpenWorldBuilderOptionsRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new OpenWorldBuilderOptionsRequest());

            if (ActionMap.ActionIs<BuildWorldRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new BuildWorldRequest());
        }
    }
}