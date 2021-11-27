using MarsUndiscovered.Messages;
using MarsUndiscovered.Messages.Console;

using DavidFidge.MonoGame.Core.UserInterface;

using InputHandlers.Keyboard;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class ConsoleKeyboardHandler : BaseKeyboardHandler
    {
        public override void HandleKeyboardKeyDown(Keys[] keysDown, Keys keyInFocus, KeyboardModifier keyboardModifier)
        {
            if (ActionMap.ActionIs<CloseConsoleRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new CloseConsoleRequest());

            if (ActionMap.ActionIs<ExecuteConsoleCommandRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new ExecuteConsoleCommandRequest());

            if (ActionMap.ActionIs<RecallConsoleHistoryBackRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new RecallConsoleHistoryBackRequest());

            if (ActionMap.ActionIs<RecallConsoleHistoryForwardRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new RecallConsoleHistoryForwardRequest());
        }
    }
}