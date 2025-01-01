using FrigidRogue.MonoGame.Core.UserInterface;
using InputHandlers.Keyboard;
using MarsUndiscovered.Messages;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class WorldBuilderOptionsKeyboardHandler : BaseKeyboardHandler
    {
        public override void HandleKeyboardKeyDown(Keys[] keysDown, Keys keyInFocus, KeyboardModifier keyboardModifier)
        {
            if (ActionMap.ActionIs<CloseWorldBuilderOptionsRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new CloseWorldBuilderOptionsRequest());

            if (ActionMap.ActionIs<QuitToTitleRequest>(keyInFocus, keyboardModifier))
            {
                Mediator.Send(new CloseWorldBuilderOptionsRequest());
                Mediator.Send(new QuitToTitleRequest());
            }
        }
    }
}