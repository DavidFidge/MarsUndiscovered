using System;

using GoRogue.GameFramework;

using MarsUndiscovered.Components;

using SadRogue.Primitives;

namespace MarsUndiscovered.Interfaces
{
    public interface IGameWorld
    {
        Player Player { get; set; }
        void Generate();
        Map Map { get; }
        Tuple<Point, Point> Move(Direction direction);
    }
}