using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.MonoGame.Core.Graphics.Camera;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarsUndiscovered.UserInterface.Views
{
    public abstract class BaseGameCoreView<TViewModel, TData> : BaseMarsUndiscoveredView<TViewModel, TData>
        where TViewModel : BaseGameCoreViewModel<TData>
        where TData : BaseGameData, new()
    {
        protected readonly IGameCamera _gameCamera;
        private RenderTarget2D _renderTarget;
        private SpriteBatch _spriteBatch;
        protected bool IsMouseInGameView => RootPanel?.IsMouseInRootPanelEmptySpace ?? true;

        protected int _mapWidth;
        protected int _mapHeight;
        
        protected BaseGameCoreView(IGameCamera gameCamera, TViewModel viewModel) : base(viewModel)
        {
            _gameCamera = gameCamera;
        }

        protected override void InitializeInternal()
        {
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            base.InitializeInternal();
        }

        protected void UpdateMapRenderTargetSize(int width, int height)
        {
            if (width != _mapWidth || height != _mapHeight)
            {
                _mapWidth = width;
                _mapHeight = height;
                
                _renderTarget = new RenderTarget2D(
                    Game.GraphicsDevice,
                    Constants.TileWidth * width,
                    Constants.TileHeight * height,
                    false,
                    Game.GraphicsDevice.PresentationParameters.BackBufferFormat,
                    Game.GraphicsDevice.PresentationParameters.DepthStencilFormat,
                    0,
                    RenderTargetUsage.PreserveContents
                );
            }
        }
        
        public override void Draw()
        {
            var oldRenderTargets = Game.GraphicsDevice.GetRenderTargets();
            
            Game.GraphicsDevice.SetRenderTarget(_renderTarget);
            Game.GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            var drawableTiles = _viewModel.MapViewModel.GetVisibleDrawableTiles();
            
            foreach (var tile in drawableTiles)
                tile.SpriteBatchDraw(_spriteBatch);
            
            _spriteBatch.End();
            
            Game.GraphicsDevice.RestoreGraphicsDeviceAfterSpriteBatchDraw();

            _viewModel.MapViewModel.SetMapEntityTexture(_renderTarget);

            Game.GraphicsDevice.SetRenderTargets(oldRenderTargets);

            _viewModel.MapViewModel.SceneGraph.Draw(_gameCamera.View, _gameCamera.Projection);
        }

        public override void Update()
        {
            _gameCamera.Update();

            base.Update();
        }

        public override void Hide()
        {
            base.Hide();
            _viewModel.IsActive = false;
        }
    }
}