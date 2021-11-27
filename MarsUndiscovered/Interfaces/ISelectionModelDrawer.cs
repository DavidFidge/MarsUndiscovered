using Microsoft.Xna.Framework;

using IDrawable = DavidFidge.MonoGame.Core.Graphics.IDrawable;

namespace MarsUndiscovered.Interfaces
{
    public interface ISelectionModelDrawer : IDrawable
    {
        BoundingBox BoundingBox { get; set; }
    }
}