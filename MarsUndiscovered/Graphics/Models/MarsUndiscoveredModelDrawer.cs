using Augmented.Interfaces;

using DavidFidge.MonoGame.Core.Graphics.Models;

using Microsoft.Xna.Framework;

namespace Augmented.Graphics.Models
{
    public class AugmentedModelDrawer : ModelDrawer, IAugmentedModelDrawer
    {
        public BoundingBox BoundingBox => Model.BoundingBox;
        public BoundingSphere BoundingSphere => Model.BoundingSphere;

        public AugmentedModelDrawer(IAssetProvider assetProvider)
        {
            Model = assetProvider.AugmentedModel;
        }
    }
}