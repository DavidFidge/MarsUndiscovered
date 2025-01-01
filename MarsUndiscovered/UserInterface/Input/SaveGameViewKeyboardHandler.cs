using FrigidRogue.MonoGame.Core.UserInterface;
using InputHandlers.Keyboard;
using MarsUndiscovered.Messages;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class SaveGameViewKeyboardHandler : BaseKeyboardHandler
    {
        public override void HandleKeyboardKeyDown(Keys[] keysDown, Keys keyInFocus, KeyboardModifier keyboardModifier)
        {
            if (ActionMap.ActionIs<CloseSaveGameViewRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new CloseSaveGameViewRequest());

            if (ActionMap.ActionIs<SaveGameRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new SaveGameRequest { FromHotkey = true });

            if (ActionMap.ActionIs<OverwriteSaveGameRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new SaveGameRequest { Overwrite = true });
        }
    }
}