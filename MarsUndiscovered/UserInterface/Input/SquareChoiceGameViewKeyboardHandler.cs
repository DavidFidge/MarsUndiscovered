using InputHandlers.Keyboard;

using MarsUndiscovered.Messages;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class SquareChoiceGameViewKeyboardHandler : BaseInventoryViewKeyboardHandler
    {
        private Keys? _ctrlArrowKey;

        public override void HandleKeyboardKeyDown(Keys[] keysDown, Keys keyInFocus, KeyboardModifier keyboardModifier)
        {
            if (ActionMap.ActionIs<CloseSquareChoiceRequest>(keyInFocus, keyboardModifier))
                Mediator.Send(new CloseSquareChoiceRequest());
            
            ProcessMoveSquare(keyInFocus, keyboardModifier);
        }

        private void ProcessMoveSquare(Keys keyInFocus, KeyboardModifier keyboardModifier)
        {
            if ((keyboardModifier & KeyboardModifier.Ctrl) == 0)
            {
                _ctrlArrowKey = null;

                if (ActionMap.ActionIs<MoveSquareChoiceSelectionUpRequest>(keyInFocus, keyboardModifier))
                    Mediator.Send(new MoveSquareChoiceSelectionUpRequest());

                if (ActionMap.ActionIs<MoveSquareChoiceSelectionUpLeftRequest>(keyInFocus, keyboardModifier))
                    Mediator.Send(new MoveSquareChoiceSelectionUpLeftRequest());

                if (ActionMap.ActionIs<MoveSquareChoiceSelectionUpRightRequest>(keyInFocus, keyboardModifier))
                    Mediator.Send(new MoveSquareChoiceSelectionUpRightRequest());

                if (ActionMap.ActionIs<MoveSquareChoiceSelectionDownRequest>(keyInFocus, keyboardModifier))
                    Mediator.Send(new MoveSquareChoiceSelectionDownRequest());

                if (ActionMap.ActionIs<MoveSquareChoiceSelectionDownLeftRequest>(keyInFocus, keyboardModifier))
                    Mediator.Send(new MoveSquareChoiceSelectionDownLeftRequest());

                if (ActionMap.ActionIs<MoveSquareChoiceSelectionDownRightRequest>(keyInFocus, keyboardModifier))
                    Mediator.Send(new MoveSquareChoiceSelectionDownRightRequest());

                if (ActionMap.ActionIs<MoveSquareChoiceSelectionLeftRequest>(keyInFocus, keyboardModifier))
                    Mediator.Send(new MoveSquareChoiceSelectionLeftRequest());

                if (ActionMap.ActionIs<MoveSquareChoiceSelectionRightRequest>(keyInFocus, keyboardModifier))
                    Mediator.Send(new MoveSquareChoiceSelectionRightRequest());
            }
            else
            {
                if (_ctrlArrowKey != null)
                {
                    if (keyInFocus != _ctrlArrowKey)
                    {
                        var keyUp = _ctrlArrowKey == Keys.Up || keyInFocus == Keys.Up;
                        var keyDown = _ctrlArrowKey == Keys.Down || keyInFocus == Keys.Down;
                        var keyLeft = _ctrlArrowKey == Keys.Left || keyInFocus == Keys.Left;
                        var keyRight = _ctrlArrowKey == Keys.Right || keyInFocus == Keys.Right;

                        if (keyUp && keyLeft)
                        {
                            Mediator.Send(new MoveSquareChoiceSelectionUpLeftRequest());
                            _ctrlArrowKey = null;
                        }
                        else if (keyDown && keyRight)
                        {
                            Mediator.Send(new MoveSquareChoiceSelectionDownRightRequest());
                            _ctrlArrowKey = null;
                        }
                        else if (keyDown && keyLeft)
                        {
                            Mediator.Send(new MoveSquareChoiceSelectionDownLeftRequest());
                            _ctrlArrowKey = null;
                        }
                        else if (keyUp && keyRight)
                        {
                            Mediator.Send(new MoveSquareChoiceSelectionUpRightRequest());
                            _ctrlArrowKey = null;
                        }
                        else
                        {
                            _ctrlArrowKey = null;
                        }
                    }
                }
                else
                {
                    _ctrlArrowKey = keyInFocus;
                }
            }
        }
    }
}