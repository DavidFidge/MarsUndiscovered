using FrigidRogue.MonoGame.Core.Interfaces.Components;
using MarsUndiscovered.Messages;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class SquareChoiceGameViewMouseHandler : BaseGameViewMouseHandler
    {
        public SquareChoiceGameViewMouseHandler(IStopwatchProvider stopwatchProvider) : base(stopwatchProvider)
        {
        }

        public override void HandleLeftMouseClick(MouseState mouseState, MouseState origin)
        {
            Mediator.Send(new LeftClickSquareChoiceGameViewRequest(mouseState.X, mouseState.Y));
        }
        
        public override void HandleMouseMoving(MouseState mouseState, MouseState origin)
        {
            if (!CanSendMouseMoveEvent())
                return;

            Mediator.Send(new SquareChoiceMouseHoverViewRequest(mouseState.X, mouseState.Y));
        }
    }
}