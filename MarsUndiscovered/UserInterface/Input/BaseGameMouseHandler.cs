using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.UserInterface;

namespace MarsUndiscovered.UserInterface.Input
{
    public abstract class BaseGameMouseHandler : BaseMouseHandler
    {
        private readonly IStopwatchProvider _stopwatchProvider;
        private double _mouseMoveThrottle = 20;
        private double _lastTotalMilliseconds;

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
    }
}