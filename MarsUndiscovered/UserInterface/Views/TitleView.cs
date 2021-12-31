using System;
using System.Threading;
using System.Threading.Tasks;

using FrigidRogue.MonoGame.Core.Extensions;

using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

using FrigidRogue.MonoGame.Core.Messages;
using FrigidRogue.MonoGame.Core.UserInterface;
using FrigidRogue.MonoGame.Core.View;
using FrigidRogue.MonoGame.Core.View.Extensions;

using GeonBit.UI.Entities;

using MediatR;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarsUndiscovered.UserInterface.Views
{
    public class TitleView : BaseMarsUndiscoveredView<TitleViewModel, TitleData>,
        IRequestHandler<OptionsButtonClickedRequest>,
        IRequestHandler<CloseOptionsViewRequest>,
        IRequestHandler<CustomGameSeedRequest>,
        IRequestHandler<CancelCustomGameSeedRequest>,
        IRequestHandler<OpenLoadGameViewRequest>,
        IRequestHandler<CloseLoadGameViewRequest>
    {
        private readonly OptionsView _optionsView;
        private readonly CustomGameSeedView _customGameSeedView;
        private readonly LoadGameView _loadGameView;

        private Panel _titleMenuPanel;

        private SpriteBatch _spriteBatch;

        public TitleView(
            TitleViewModel titleViewModel,
            OptionsView optionsView,
            CustomGameSeedView customGameSeedView,
            LoadGameView loadGameView
            )
            : base(titleViewModel)
        {
            _optionsView = optionsView;
            _customGameSeedView = customGameSeedView;
            _loadGameView = loadGameView;
        }

        protected override void InitializeInternal()
        {
            _titleMenuPanel = new Panel()
                .AutoHeight()
                .WidthOfButtonWithPadding()
                .Opacity90Percent();

            _titleMenuPanel.Anchor = Anchor.BottomRight;
            _titleMenuPanel.Offset = new Vector2(200f, 200f);

            RootPanel.AddChild(_titleMenuPanel);

            new Button("New Game")
                .SendOnClick<NewGameRequest>(Mediator)
                .AddTo(_titleMenuPanel);

            SetupChildPanelWithButton<CustomGameSeedRequest, CustomGameSeedViewModel, CustomGameSeedData>(
                _titleMenuPanel,
                "New Game (custom seed)",
                _customGameSeedView);

            SetupChildPanelWithButton<OpenLoadGameViewRequest, LoadGameViewModel, LoadGameData>(
                _titleMenuPanel,
                "Load Game",
                _loadGameView);

            SetupChildPanelWithButton<OptionsButtonClickedRequest, OptionsViewModel, OptionsData>(
                _titleMenuPanel,
                "Options",
                _optionsView);

            new Button("Exit")
                .SendOnClick<QuitToDesktopRequest>(Mediator)
                .AddTo(_titleMenuPanel);

            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
        }

        public Task<Unit> Handle(CustomGameSeedRequest request, CancellationToken cancellationToken)
        {
            return ShowChildView(_customGameSeedView, _titleMenuPanel);
        }

        public Task<Unit> Handle(CancelCustomGameSeedRequest request, CancellationToken cancellationToken)
        {
            return HideChildView(_customGameSeedView, _titleMenuPanel);
        }

        public Task<Unit> Handle(OptionsButtonClickedRequest request, CancellationToken cancellationToken)
        {
            return ShowChildView(_optionsView, _titleMenuPanel);
        }

        public Task<Unit> Handle(CloseOptionsViewRequest request, CancellationToken cancellationToken)
        {
            return HideChildView(_optionsView, _titleMenuPanel);
        }

        public Task<Unit> Handle(OpenLoadGameViewRequest request, CancellationToken cancellationToken)
        {
            return ShowChildView(_loadGameView, _titleMenuPanel);
        }

        public Task<Unit> Handle(CloseLoadGameViewRequest request, CancellationToken cancellationToken)
        {
            return HideChildView(_loadGameView, _titleMenuPanel);
        }

        public override void Draw()
        {
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            _spriteBatch.Draw(Assets.TitleTexture, Game.GraphicsDevice.ViewportRectangle(), Color.White);
            _spriteBatch.End();
        }
    }
}