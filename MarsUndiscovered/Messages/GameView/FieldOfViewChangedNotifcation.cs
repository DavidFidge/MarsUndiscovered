using System.Collections.Generic;
using System.Linq;

using MediatR;
using SadRogue.Primitives;

namespace MarsUndiscovered.Messages
{
    public class FieldOfViewChangedNotifcation : INotification
    {
        public IEnumerable<Point> NewlyVisiblePoints { get; }
        public IEnumerable<Point> NewlyHiddenPoints { get; }

        public FieldOfViewChangedNotifcation(IEnumerable<Point> newlyVisiblePoints, IEnumerable<Point> newlyHiddenPoints)
        {
            NewlyVisiblePoints = newlyVisiblePoints.ToList();
            NewlyHiddenPoints = newlyHiddenPoints.ToList();
        }
    }
}