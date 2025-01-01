using FrigidRogue.MonoGame.Core.Messages;
using FrigidRogue.MonoGame.Core.UserInterface;
using InputHandlers.Keyboard;
using MarsUndiscovered.Messages;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class TitleViewKeyboardHandler : BaseKeyboardHandler
    {
        public override void HandleKeyboardKeyDown(Keys[] keysDown, Keys keyInFocus, KeyboardModifier keyboardModifier)
        {
            if (ActionMap.ActionIs<QuitToDesktopRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new QuitToDesktopRequest());

            if (ActionMap.ActionIs<NewGameRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new NewGameRequest());

            if (ActionMap.ActionIs<CustomGameSeedRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new CustomGameSeedRequest());
            
            if (ActionMap.ActionIs<WorldBuilderRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new WorldBuilderRequest());
        }
    }
}