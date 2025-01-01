
using FrigidRogue.MonoGame.Core.Components.Mediator;

namespace MarsUndiscovered.Messages
{
    public class MouseHoverViewNotification : INotification
    {
        public MouseHoverViewNotification(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }
    }
}