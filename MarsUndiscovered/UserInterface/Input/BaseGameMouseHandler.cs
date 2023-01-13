using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.UserInterface;
using MarsUndiscovered.UserInterface.Input.CameraMovementSpace;
using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public abstract class BaseGameMouseHandler : BaseMouseHandler
    {
        private readonly IStopwatchProvider _stopwatchProvider;
        private double _mouseMoveThrottle = 20;
        private double _lastTotalMilliseconds;

        public ICameraMovement CameraMovement { get; set; }

        public BaseGameMouseHandler(IStopwatchProvider stopwatchProvider)
        {
            _stopwatchProvider = stopwatchProvider;
            _stopwatchProvider.Start();
        }

        protected bool CanSendMouseMoveEvent()
        {
            if (_stopwatchProvider.Elapsed.TotalMilliseconds - _lastTotalMilliseconds > _mouseMoveThrottle)
            {
                _lastTotalMilliseconds = _stopwatchProvider.Elapsed.TotalMilliseconds;
                return true;
            }

            return false;
        }

        public override void HandleMouseScrollWheelMove(MouseState mouseState, int difference)
        {
            CameraMovement.ZoomCamera(difference);
            
            base.HandleMouseScrollWheelMove(mouseState, difference);
        }
    }
}