using MarsUndiscovered.Messages;

using DavidFidge.MonoGame.Core.Interfaces.Components;
using DavidFidge.MonoGame.Core.UserInterface;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class GameViewMouseHandler : BaseMouseHandler
    {
        private readonly IGameProvider _gameProvider;
        private MouseState? _mouseStateBeforeRotation;
        private int _halvedWindowX;
        private int _halvedWindowY;

        public GameViewMouseHandler(IGameProvider gameProvider)
        {
            _gameProvider = gameProvider;
        }

        public override void HandleRightMouseDragging(MouseState mouseState, MouseState originalMouseState)
        {
            if (_mouseStateBeforeRotation == null)
            {
                _mouseStateBeforeRotation = originalMouseState;

                _halvedWindowX = _gameProvider.Game.Window.ClientBounds.Width / 2;
                _halvedWindowY = _gameProvider.Game.Window.ClientBounds.Height / 2;
            }
            else
            {
                var xDisplacement = mouseState.X - _halvedWindowX;
                var yDisplacement = mouseState.Y - _halvedWindowY;

                Mediator.Send(new Rotate3DViewRequest(-xDisplacement / 100f, yDisplacement / 100f));
            }

            Mouse.SetPosition(_halvedWindowX, _halvedWindowY);
        }

        public override void HandleRightMouseDragDone(MouseState mouseState, MouseState originalMouseState)
        {
            if (_mouseStateBeforeRotation != null)
                Mouse.SetPosition(originalMouseState.X, originalMouseState.Y);

            _mouseStateBeforeRotation = null;

            //Mouse.SetCursor(MouseCursor.Arrow);
            base.HandleRightMouseDragDone(mouseState, originalMouseState);
        }

        public override void HandleMouseScrollWheelMove(MouseState mouseState, int difference)
        {
            Mediator.Send(new Zoom3DViewRequest(difference));
        }

        public override void HandleLeftMouseClick(MouseState mouseState, MouseState origin)
        {
            Mediator.Send(new Select3DViewRequest(mouseState.X,
                mouseState.Y));
        }

        public override void HandleLeftMouseDoubleClick(MouseState mouseState, MouseState origin)
        {
            Mediator.Send(new Select3DViewRequest(mouseState.X,
                mouseState.Y));
        }

        public override void HandleRightMouseClick(MouseState mouseState, MouseState origin)
        {
            Mediator.Send(new Action3DViewRequest(mouseState.X,
                mouseState.Y));
        }

        public override void HandleRightMouseDoubleClick(MouseState mouseState, MouseState origin)
        {
            Mediator.Send(new Action3DViewRequest(mouseState.X,
                mouseState.Y));
        }
    }
}