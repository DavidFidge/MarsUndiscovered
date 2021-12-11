using FrigidRogue.MonoGame.Core.Interfaces.Graphics;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Interfaces
{
    public interface IGameWorld
    {
        void Generate();
        ArrayView<bool> WallsFloors { get; set; }
    }
}