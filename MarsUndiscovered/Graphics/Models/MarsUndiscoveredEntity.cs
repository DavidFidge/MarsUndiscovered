using Augmented.Interfaces;

using DavidFidge.MonoGame.Core.Components;
using DavidFidge.MonoGame.Core.Graphics;
using DavidFidge.MonoGame.Core.Interfaces.Components;

using Microsoft.Xna.Framework;

using IDrawable = DavidFidge.MonoGame.Core.Graphics.IDrawable;

namespace Augmented.Graphics.Models
{
    public class AugmentedEntity : Entity, IDrawable, ISelectable
    {
        protected readonly IGameProvider _gameProvider;
        private readonly IAugmentedModelDrawer _augmentedModelDrawer;
        private readonly ISelectionModelDrawer _selectionModelDrawer;

        public bool IsSelected { get; set; }

        public AugmentedEntity(
            IGameProvider gameProvider,
            IAugmentedModelDrawer augmentedModelDrawer,
            ISelectionModelDrawer selectionModelDrawer)
        {
            _gameProvider = gameProvider;
            _augmentedModelDrawer = augmentedModelDrawer;
            _selectionModelDrawer = selectionModelDrawer;

            _selectionModelDrawer.BoundingBox = _augmentedModelDrawer.BoundingBox;

            LocalTransform.ChangeTranslation(new Vector3(0, 0, _augmentedModelDrawer.BoundingBox.Max.Z - _augmentedModelDrawer.BoundingBox.Min.Z) / 2f);

        }

        public void Draw(Matrix view, Matrix projection, Matrix world)
        {
            if (IsSelected)
                _selectionModelDrawer.Draw(view, projection, world);

            _augmentedModelDrawer.Draw(view, projection, world);
        }

        public float? Intersects(Ray ray, Matrix worldTransform)
        {
            return ray.Intersects(_augmentedModelDrawer.BoundingSphere.Transform(worldTransform));
        }
    }
}
