using DavidFidge.MonoGame.Core.Interfaces.Graphics;

using Microsoft.Xna.Framework;

namespace MarsUndiscovered.Interfaces
{
    public interface IMarsUndiscoveredModelDrawer : IModelDrawer
    {
        BoundingBox BoundingBox { get; }
        BoundingSphere BoundingSphere { get; }
    }
}