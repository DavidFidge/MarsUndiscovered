using MarsUndiscovered.Interfaces;

using DavidFidge.MonoGame.Core.Components;
using DavidFidge.MonoGame.Core.Graphics;
using DavidFidge.MonoGame.Core.Interfaces.Components;

using Microsoft.Xna.Framework;

using IDrawable = DavidFidge.MonoGame.Core.Graphics.IDrawable;

namespace MarsUndiscovered.Graphics.Models
{
    public class MarsUndiscoveredEntity : Entity, IDrawable, ISelectable
    {
        protected readonly IGameProvider _gameProvider;
        private readonly IMarsUndiscoveredModelDrawer _marsUndiscoveredModelDrawer;
        private readonly ISelectionModelDrawer _selectionModelDrawer;

        public bool IsSelected { get; set; }

        public MarsUndiscoveredEntity(
            IGameProvider gameProvider,
            IMarsUndiscoveredModelDrawer marsUndiscoveredModelDrawer,
            ISelectionModelDrawer selectionModelDrawer)
        {
            _gameProvider = gameProvider;
            _marsUndiscoveredModelDrawer = marsUndiscoveredModelDrawer;
            _selectionModelDrawer = selectionModelDrawer;

            _selectionModelDrawer.BoundingBox = _marsUndiscoveredModelDrawer.BoundingBox;

            LocalTransform.ChangeTranslation(new Vector3(0, 0, _marsUndiscoveredModelDrawer.BoundingBox.Max.Z - _marsUndiscoveredModelDrawer.BoundingBox.Min.Z) / 2f);

        }

        public void Draw(Matrix view, Matrix projection, Matrix world)
        {
            if (IsSelected)
                _selectionModelDrawer.Draw(view, projection, world);

            _marsUndiscoveredModelDrawer.Draw(view, projection, world);
        }

        public float? Intersects(Ray ray, Matrix worldTransform)
        {
            return ray.Intersects(_marsUndiscoveredModelDrawer.BoundingSphere.Transform(worldTransform));
        }
    }
}
