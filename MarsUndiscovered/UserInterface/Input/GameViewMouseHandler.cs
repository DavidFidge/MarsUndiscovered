using MarsUndiscovered.Messages;

using FrigidRogue.MonoGame.Core.Interfaces.Components;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class GameViewMouseHandler : BaseGameViewMouseHandler
    {
        public GameViewMouseHandler(IStopwatchProvider stopwatchProvider) : base(stopwatchProvider)
        {
        }

        public override void HandleLeftMouseClick(MouseState mouseState, MouseState origin)
        {
            Mediator.Send(new LeftClickViewRequest(mouseState.X, mouseState.Y));
        }

        public override void HandleLeftMouseDoubleClick(MouseState mouseState, MouseState origin)
        {
            Mediator.Send(new LeftClickViewRequest(mouseState.X, mouseState.Y));
        }

        public override void HandleRightMouseClick(MouseState mouseState, MouseState origin)
        {
            Mediator.Send(new RightClickViewRequest(mouseState.X, mouseState.Y));
        }

        public override void HandleRightMouseDoubleClick(MouseState mouseState, MouseState origin)
        {
            Mediator.Send(new RightClickViewRequest(mouseState.X, mouseState.Y));
        }

        public override void HandleMouseMoving(MouseState mouseState, MouseState origin)
        {
            if (!CanSendMouseMoveEvent())
                return;

            Mediator.Publish(new MouseHoverViewNotification(mouseState.X, mouseState.Y));
        }
    }
}