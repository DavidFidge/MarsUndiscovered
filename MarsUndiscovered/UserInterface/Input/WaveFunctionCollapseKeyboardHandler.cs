using MarsUndiscovered.Messages;
using InputHandlers.Keyboard;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class WaveFunctionCollapseKeyboardHandler : BaseGameViewKeyboardHandler
    {
        public override void HandleKeyboardKeyDown(Keys[] keysDown, Keys keyInFocus, KeyboardModifier keyboardModifier)
        {
            base.HandleKeyboardKeyDown(keysDown, keyInFocus, keyboardModifier);

            if (ActionMap.ActionIs<NextStepRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new NextStepRequest());

            if (ActionMap.ActionIs<MewMapRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new MewMapRequest());

            if (ActionMap.ActionIs<PlayUntilCompleteRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new PlayUntilCompleteRequest());
        }
    }
}