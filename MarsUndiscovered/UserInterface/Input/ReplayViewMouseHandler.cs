using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.UserInterface;

using MarsUndiscovered.Messages;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.UserInterface.Input
{
    public class ReplayViewMouseHandler : BaseGameMouseHandler
    {
        public ReplayViewMouseHandler(IStopwatchProvider stopwatchProvider) : base(stopwatchProvider)
        {
        }

        public override void HandleMouseMoving(MouseState mouseState, MouseState origin)
        {
            if (!CanSendMouseMoveEvent())
                return;

            Mediator.Publish(new MouseHoverViewNotification(mouseState.X, mouseState.Y));
        }
    }
}