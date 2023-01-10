using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Input.CameraMovementSpace;

using InputHandlers.Keyboard;
using MarsUndiscovered.Components;
using MarsUndiscovered.UserInterface.Views;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class WorldBuilderViewKeyboardHandler : BaseGameViewKeyboardHandler
    {
        public override void HandleKeyboardKeyDown(Keys[] keysDown, Keys keyInFocus, KeyboardModifier keyboardModifier)
        {
            base.HandleKeyboardKeyDown(keysDown, keyInFocus, keyboardModifier);

            if (ActionMap.ActionIs<OpenWorldBuilderOptionsRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new OpenWorldBuilderOptionsRequest());

            if (ActionMap.ActionIs<BuildWorldRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new BuildWorldRequest { WorldGenerationTypeParams = new WorldGenerationTypeParams(MapType.Outdoor)});
            
            if (ActionMap.ActionIs<NextWorldBuilderStepRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new NextWorldBuilderStepRequest());
            
            if (ActionMap.ActionIs<PreviousWorldBuilderStepRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new PreviousWorldBuilderStepRequest());
        }
    }
}