using System.Threading;
using System.Threading.Tasks;
using FrigidRogue.MonoGame.Core.Extensions;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;
using FrigidRogue.MonoGame.Core.View.Extensions;

using GeonBit.UI.Entities;
using MarsUndiscovered.Game.Components.WaveFunction;
using MarsUndiscovered.Messages;
using MediatR;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarsUndiscovered.UserInterface.Views
{
    public class WaveFunctionCollapseView : BaseMarsUndiscoveredView<WaveFunctionCollapseViewModel, WaveFunctionCollapseData>,
        IRequestHandler<NextStepRequest>,
        IRequestHandler<MewMapRequest>,
        IRequestHandler<PlayUntilCompleteRequest>
    {
        private Panel _leftPanel;
        private Panel _mapPanel;
        private SpriteBatch _spriteBatch;
        private WaveFunctionCollapse _waveFunctionCollapse;
        private int _mapHeight;
        private int _mapWidth;
        private bool _playUntilComplete;

        public WaveFunctionCollapseView(WaveFunctionCollapseViewModel waveFunctionCollapseViewModel)
            : base(waveFunctionCollapseViewModel)
        {
        }

        protected override void InitializeInternal()
        {
            _leftPanel = new Panel()
                .Anchor(Anchor.TopLeft)
                .Width(Constants.LeftPanelWidth)
                .SkinNone()
                .NoPadding()
                .Height(0.999f);

            _mapPanel = new Panel()
                .Anchor(Anchor.TopRight)
                .Width(Constants.MiddlePanelWidth)
                .SkinNone()
                .NoPadding()
                .Height(0.999f);

            new Button("New Map")
                .SendOnClick<MewMapRequest>(Mediator)
                .AddTo(_leftPanel);

            new Button("Next Step")
                .SendOnClick<NextStepRequest>(Mediator)
                .AddTo(_leftPanel);

            new Button("Play Until Complete")
                .SendOnClick<PlayUntilCompleteRequest>(Mediator)
                .AddTo(_leftPanel);

            new Button("Exit")
                .SendOnClick<QuitToTitleRequest>(Mediator)
                .AddTo(_leftPanel);

            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            _waveFunctionCollapse = new WaveFunctionCollapse();
            _waveFunctionCollapse.CreateTiles(GameProvider.Game.Content);

            _mapWidth = 40;
            _mapHeight = 40;

            _waveFunctionCollapse.Reset(_mapWidth, _mapHeight);
        }

        public override void Draw()
        {
            var drawOrigin = new Vector2(300, 0);
            var tileSize = new Vector2(Game.GraphicsDevice.ViewportRectangle().Width - drawOrigin.X / _mapWidth, Game.GraphicsDevice.ViewportRectangle().Height - drawOrigin.Y / _mapHeight);

            _spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend);

            var tiles = _waveFunctionCollapse.CurrentState;

            foreach (var tile in tiles)
            {
                if (tile.IsCollapsed)
                {
                    var drawLocation = drawOrigin + (tileSize * new Vector2(tile.Point.X, tile.Point.Y));
                    _spriteBatch.Draw(tile.TileChoice.Texture, drawLocation, tile.TileChoice.Texture.Bounds, Color.White);
                }
            }

            _spriteBatch.End();
        }

        public Task<Unit> Handle(NextStepRequest request, CancellationToken cancellationToken)
        {
            _waveFunctionCollapse.NextStep();
            return Unit.Task;
        }

        public Task<Unit> Handle(PlayUntilCompleteRequest request, CancellationToken cancellationToken)
        {
            _playUntilComplete = !_playUntilComplete;
            return Unit.Task;
        }

        public Task<Unit> Handle(MewMapRequest request, CancellationToken cancellationToken)
        {
            _waveFunctionCollapse.Reset(_mapWidth, _mapHeight);

            return Unit.Task;
        }

        public override void Update()
        {
            if (_playUntilComplete)
            {
                var result = _waveFunctionCollapse.NextStep();

                if (result.IsComplete)
                {
                    _playUntilComplete = false;
                }
                else if (result.IsFailed)
                {
                    _waveFunctionCollapse.Reset(_mapWidth, _mapHeight);
                }
            }

            base.Update();
        }
    }
}