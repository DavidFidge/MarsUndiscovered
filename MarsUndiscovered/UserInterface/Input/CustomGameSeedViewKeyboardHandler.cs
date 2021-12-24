using FrigidRogue.MonoGame.Core.Messages;
using FrigidRogue.MonoGame.Core.UserInterface;

using InputHandlers.Keyboard;

using MarsUndiscovered.Messages;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class CustomGameSeedViewKeyboardHandler : BaseKeyboardHandler
    {
        public override void HandleKeyboardKeyDown(Keys[] keysDown, Keys keyInFocus, KeyboardModifier keyboardModifier)
        {
            if (ActionMap.ActionIs<CancelCustomGameSeedRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new CancelCustomGameSeedRequest());

            if (ActionMap.ActionIs<CustomGameSeedEnterKeyRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new CustomGameSeedEnterKeyRequest());
        }
    }
}