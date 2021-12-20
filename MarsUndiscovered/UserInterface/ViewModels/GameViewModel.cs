using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Graphics;
using FrigidRogue.MonoGame.Core.UserInterface;

using MarsUndiscovered.Interfaces;
using MarsUndiscovered.UserInterface.Data;

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

            _mapEntity = MapEntityFactory.Create();

            _mapEntity.CreateTranslation(GameWorld.Map.Width, GameWorld.Map.Height, Graphics.Assets.TileQuadWidth, Graphics.Assets.TileQuadHeight);

            SceneGraph.Initialise(_mapEntity);

            for (var x = 0; x < GameWorld.Map.Width; x++)
            {
                for (var y = 0; y < GameWorld.Map.Height; y++)
                {
                    var mapTileEntity = MapTileEntityFactory.Create();

                    mapTileEntity.Initialize(x, y, GameWorld.Map.Terrain);

                    SceneGraph.Add(mapTileEntity, _mapEntity);
                }
            }
        }
    }
}