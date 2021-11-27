using Microsoft.Xna.Framework;

using IDrawable = DavidFidge.MonoGame.Core.Graphics.IDrawable;

namespace Augmented.Interfaces
{
    public interface ISelectionModelDrawer : IDrawable
    {
        BoundingBox BoundingBox { get; set; }
    }
}