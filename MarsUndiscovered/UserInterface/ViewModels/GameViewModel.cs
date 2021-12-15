using FrigidRogue.MonoGame.Core.Interfaces.Graphics;
using FrigidRogue.MonoGame.Core.UserInterface;

using MarsUndiscovered.Components;
using MarsUndiscovered.Interfaces;
using MarsUndiscovered.UserInterface.Data;

using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class GameViewModel : BaseViewModel<GameData>
    {
        private MapEntity _mapEntity;
        public IGameWorld GameWorld { get; set; }
        public ISceneGraph SceneGraph { get; set; }
        public IAssets Assets { get; set; }
        public IFactory<MapTileEntity> MapTileEntityFactory { get; set; }
        public IFactory<MapEntity> MapEntityFactory { get; set; }

        public override void Initialize()
        {
            base.Initialize();

            GameWorld.Generate();

            Data.WallsFloors = (ArrayView<bool>)GameWorld.WallsFloors.Clone();

            _mapEntity = MapEntityFactory.Create();

            _mapEntity.CreateTranslation(GameWorld.WallsFloors.Width, GameWorld.WallsFloors.Height, Graphics.Assets.TileQuadWidth, Graphics.Assets.TileQuadHeight);

            SceneGraph.Initialise(_mapEntity);

            for (var x = 0; x < Data.WallsFloors.Width; x++)
            {
                for (var y = 0; y < Data.WallsFloors.Height; y++)
                {
                    var mapTileEntity = MapTileEntityFactory.Create();

                    mapTileEntity.Initialize(x, y, Data.WallsFloors);

                    SceneGraph.Add(mapTileEntity, _mapEntity);
                }
            }
        }
    }
}