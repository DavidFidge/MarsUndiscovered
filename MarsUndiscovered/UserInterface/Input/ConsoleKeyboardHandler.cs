using Augmented.Messages;
using Augmented.Messages.Console;

using DavidFidge.MonoGame.Core.UserInterface;

using InputHandlers.Keyboard;

using Microsoft.Xna.Framework.Input;

namespace Augmented.UserInterface.Input
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