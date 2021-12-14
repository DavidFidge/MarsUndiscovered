using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Graphics;
using FrigidRogue.MonoGame.Core.Graphics.Quads;
using FrigidRogue.MonoGame.Core.Interfaces.Graphics;
using MarsUndiscovered.UserInterface.Data;

using FrigidRogue.MonoGame.Core.UserInterface;
using GoRogue.DiceNotation.Terms;
using MarsUndiscovered.Interfaces;
using Microsoft.Xna.Framework;
using SadRogue.Primitives.GridViews;
using IDrawable = FrigidRogue.MonoGame.Core.Graphics.IDrawable;
using Point = SadRogue.Primitives.Point;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class GameViewModel : BaseViewModel<GameData>
    {
        private RootSceneGraphEntity _rootSceneGraphEntity;
        public IGameWorld GameWorld { get; set; }
        public ISceneGraph SceneGraph { get; set; }
        public IAssets Assets { get; set; }

        public override void Initialize()
        {
            base.Initialize();

            GameWorld.Generate();

            Data.WallsFloors = (ArrayView<bool>)GameWorld.WallsFloors.Clone();

            _rootSceneGraphEntity = new RootSceneGraphEntity();

            SceneGraph.Initialise(_rootSceneGraphEntity);

            for (var x = 0; x < Data.WallsFloors.Width; x++)
            {
                for (var y = 0; y < Data.WallsFloors.Height; y++)
                {
                    var mapTileEntity = new MapTileEntity(Assets)
                    {
                        X = x,
                        Y = y,
                        Index = Point.ToIndex(x, y, Data.WallsFloors.Width),
                        MaxHeight = Data.WallsFloors.Height
                    };

                    mapTileEntity.IsFloor = Data.WallsFloors[x, y];
                    mapTileEntity.IsWall = !Data.WallsFloors[x, y];
        
                    SceneGraph.Add(mapTileEntity, _rootSceneGraphEntity);
                }
            }
        }
    }

    public class MapTileEntity : Entity, IDrawable
    {
        private IAssets _assets;
        public int X { get; set; }
        public int Y { get; set; }
        public int Index { get; set; }

        public int MaxHeight { get; set; }

        public float CellSize => 2f / MaxHeight;

        public bool IsFloor { get; set; }
        public bool IsWall { get; set; }

        public MapTileEntity(IAssets assets)
        {
            _assets = assets;
        }

        public void Initialize()
        {
            //var scale = Matrix.CreateScale(CellSize);
            //var localTranslation = Matrix.CreateTranslation(X * CellSize, Y * CellSize, 0);

            //var worldTranslation = Matrix.CreateTranslation(-1, -1, -1);
            //var transform = Matrix.Multiply(Matrix.Multiply(scale, localTranslation), worldTranslation);

            Transform.ChangeTranslation(new Vector3(X * CellSize, Y * CellSize, 0));
            Transform.ChangeScale(new Vector3(CellSize, CellSize, CellSize));
        }

        public void Draw(Matrix view, Matrix projection, Matrix world)
        {
            //WallBackgroundQuad?.Draw(view, projection, world);
            var scale = Matrix.CreateScale(CellSize);
            var localTranslation = Matrix.CreateTranslation(X * CellSize, Y * CellSize, 0);
            var worldTranslation = Matrix.CreateTranslation(-1, -1, -1);
            var transform = Matrix.Multiply(Matrix.Multiply(scale, localTranslation), worldTranslation);

            if (IsWall)
            {
                //_assets.WallBackgroundQuad?.Effect.Parameters["Colour"].SetValue(Color.Red.ToVector4());
                //_assets.WallForegroundQuad?.Effect.Parameters["Colour"].SetValue(Color.Red.ToVector4());
                _assets.WallBackgroundQuad?.Draw(view, projection, transform);
                _assets.WallForegroundQuad?.Draw(view, projection, transform);
            }
            else
            {
                _assets.FloorQuad?.Draw(view, projection, transform);
            }
        }
    }

    public class RootSceneGraphEntity : Entity
    {
        public RootSceneGraphEntity()
        {
            Transform.ChangeTranslation(new Vector3(-1, -1, -1));
        }
    }
}