using FrigidRogue.MonoGame.Core.Graphics.Camera;
using MarsUndiscovered.Components;
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
        
        protected BaseGameCoreView(IGameCamera gameCamera, TViewModel viewModel) : base(viewModel)
        {
            _gameCamera = gameCamera;
        }

        protected override void InitializeInternal()
        {
            _renderTarget = new RenderTarget2D(
                Game.GraphicsDevice,
                Constants.TileWidth * MarsMap.MapWidth,
                Constants.TileHeight * MarsMap.MapHeight,
                false,
                Game.GraphicsDevice.PresentationParameters.BackBufferFormat,
                Game.GraphicsDevice.PresentationParameters.DepthStencilFormat,
                0,
                RenderTargetUsage.PreserveContents
            );

            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            base.InitializeInternal();
        }
        
        public override void Draw()
        {
            var oldDepthStencilState = Game.GraphicsDevice.DepthStencilState;
            var oldRenderTargets = Game.GraphicsDevice.GetRenderTargets();
            
            Game.GraphicsDevice.SetRenderTarget(_renderTarget);
            Game.GraphicsDevice.DepthStencilState = DepthStencilState.None;
            Game.GraphicsDevice.Clear(Color.Black);
            
            _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            var drawableTiles = _viewModel.MapViewModel.GetVisibleDrawableTiles();
            
            foreach (var tile in drawableTiles)
                tile.SpriteBatchDraw(_spriteBatch);
            
            _spriteBatch.End();
            
            _viewModel.MapViewModel.SetMapEntityTexture(_renderTarget);

            Game.GraphicsDevice.DepthStencilState = oldDepthStencilState;
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