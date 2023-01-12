using MarsUndiscovered.Messages;

using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Messages;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class GameViewMouseHandler : BaseGameMouseHandler
    {
        private MouseState? _mouseStateBeforeDrag;
        private int _halvedWindowX;
        private int _halvedWindowY;

        public GameViewMouseHandler(IStopwatchProvider stopwatchProvider) : base(stopwatchProvider)
        {
        }

        public override void HandleRightMouseDragging(MouseState mouseState, MouseState originalMouseState)
        {
            if (_mouseStateBeforeDrag == null)
            {
                _mouseStateBeforeDrag = originalMouseState;

                _halvedWindowX = Game.Window.ClientBounds.Width / 2;
                _halvedWindowY = Game.Window.ClientBounds.Height / 2;
            }
            else
            {
                var xDisplacement = mouseState.X - _halvedWindowX;
                var yDisplacement = mouseState.Y - _halvedWindowY;

                Mediator.Send(new MoveViewRequest(xDisplacement * 0.05f, yDisplacement * 0.05f));
            }

            Mouse.SetPosition(_halvedWindowX, _halvedWindowY);
        }

        public override void HandleRightMouseDragDone(MouseState mouseState, MouseState originalMouseState)
        {
            if (_mouseStateBeforeDrag != null)
                Mouse.SetPosition(originalMouseState.X, originalMouseState.Y);

            _mouseStateBeforeDrag = null;

            base.HandleRightMouseDragDone(mouseState, originalMouseState);
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