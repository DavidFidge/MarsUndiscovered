using DavidFidge.MonoGame.Core.Interfaces.Graphics;

using Microsoft.Xna.Framework;

namespace Augmented.Interfaces
{
    public interface IAugmentedModelDrawer : IModelDrawer
    {
        BoundingBox BoundingBox { get; }
        BoundingSphere BoundingSphere { get; }
    }
}