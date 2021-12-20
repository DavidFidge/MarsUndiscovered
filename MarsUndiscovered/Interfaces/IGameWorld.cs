using GoRogue.GameFramework;
using MarsUndiscovered.Components;

using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Interfaces
{
    public interface IGameWorld
    {
        Player Player { get; set; }
        void Generate();
        Map Map { get; }
    }
}