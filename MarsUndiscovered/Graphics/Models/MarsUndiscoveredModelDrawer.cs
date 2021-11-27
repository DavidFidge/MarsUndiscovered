using MarsUndiscovered.Interfaces;

using DavidFidge.MonoGame.Core.Graphics.Models;

using Microsoft.Xna.Framework;

namespace MarsUndiscovered.Graphics.Models
{
    public class MarsUndiscoveredModelDrawer : ModelDrawer, IMarsUndiscoveredModelDrawer
    {
        public BoundingBox BoundingBox => Model.BoundingBox;
        public BoundingSphere BoundingSphere => Model.BoundingSphere;

        public MarsUndiscoveredModelDrawer(IAssetProvider assetProvider)
        {
            Model = assetProvider.MarsUndiscoveredModel;
        }
    }
}