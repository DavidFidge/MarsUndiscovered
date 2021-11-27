using MarsUndiscovered.Interfaces;

using DavidFidge.MonoGame.Core.Graphics.Extensions;

using Microsoft.Xna.Framework;

namespace MarsUndiscovered.Graphics.Models
{
    public class SelectionModelDrawer : ISelectionModelDrawer
    {
        private SelectionModel _selectionModel;

        public BoundingBox BoundingBox { get; set; }

        public SelectionModelDrawer(IAssetProvider assetProvider)
        {
            BoundingBox = new BoundingBox();
            _selectionModel = assetProvider.SelectionModel;
        }

        public void Draw(Matrix view, Matrix projection, Matrix world)
        {
            _selectionModel.SelectionColour = Color.Yellow;

            var selectionQuadTransform = world;

            selectionQuadTransform.Translation +=
                new Vector3(
                    BoundingBox.Min.X + ((BoundingBox.Max.X - BoundingBox.Min.X) / 2),
                    BoundingBox.Min.Y + ((BoundingBox.Max.Y - BoundingBox.Min.Y) / 2),
                    BoundingBox.Min.Z);

            var scaleVector = new Vector3(BoundingBox.Width(), BoundingBox.Length(), 1f) * new Vector3(2f);

            selectionQuadTransform = scaleVector.CreateScale() * selectionQuadTransform;

            _selectionModel.Draw(view, projection, selectionQuadTransform);
        }
    }
}