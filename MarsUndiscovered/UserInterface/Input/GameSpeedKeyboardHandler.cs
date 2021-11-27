using MarsUndiscovered.Messages;

using DavidFidge.MonoGame.Core.UserInterface;

using InputHandlers.Keyboard;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class GameSpeedKeyboardHandler : BaseKeyboardHandler
    {
        public override void HandleKeyboardKeyDown(Keys[] keysDown, Keys keyInFocus, KeyboardModifier keyboardModifier)
        {
            if (ActionMap.ActionIs<ChangeGameSpeedRequest>(keyInFocus, keyboardModifier, "Decrease Game Speed"))
            {
                Mediator.Send(new ChangeGameSpeedRequest().DecreaseSpeedRequest());
            }
            else if (ActionMap.ActionIs<ChangeGameSpeedRequest>(keyInFocus, keyboardModifier, "Increase Game Speed"))
            {
                Mediator.Send(new ChangeGameSpeedRequest().IncreaseSpeedRequest());
            }
            else if (ActionMap.ActionIs<ChangeGameSpeedRequest>(keyInFocus, keyboardModifier, "Pause Game"))
            {
                Mediator.Send(new ChangeGameSpeedRequest().TogglePauseGameRequest());
            }
        }
    }
}