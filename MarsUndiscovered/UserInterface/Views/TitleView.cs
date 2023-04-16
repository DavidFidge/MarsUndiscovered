using System.Threading;
using System.Threading.Tasks;

using FrigidRogue.MonoGame.Core.Extensions;

using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

using FrigidRogue.MonoGame.Core.Messages;
using FrigidRogue.MonoGame.Core.View.Extensions;

using GeonBit.UI.Entities;

using MediatR;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarsUndiscovered.UserInterface.Views
{
    public class TitleView : BaseMarsUndiscoveredView<TitleViewModel, TitleData>,
        IRequestHandler<OptionsButtonClickedRequest>,
        IRequestHandler<DeveloperToolsButtonClickedRequest>,
        IRequestHandler<CloseOptionsViewRequest>,
        IRequestHandler<CloseDeveloperToolsViewRequest>,
        IRequestHandler<CustomGameSeedRequest>,
        IRequestHandler<CancelCustomGameSeedRequest>,
        IRequestHandler<OpenLoadGameViewRequest>,
        IRequestHandler<CloseLoadGameViewRequest>,
        IRequestHandler<OpenLoadReplayViewRequest>,
        IRequestHandler<CloseLoadReplayViewRequest>
    {
        private readonly OptionsView _optionsView;
        private readonly CustomGameSeedView _customGameSeedView;
        private readonly LoadGameView _loadGameView;
        private readonly LoadReplayView _loadReplayView;
        private readonly DeveloperToolsView _developerToolsView;

        private Panel _titleMenuPanel;

        private SpriteBatch _spriteBatch;

        public TitleView(
            TitleViewModel titleViewModel,
            OptionsView optionsView,
            CustomGameSeedView customGameSeedView,
            LoadGameView loadGameView,
            LoadReplayView loadReplayView,
            DeveloperToolsView developerToolsView
            )
            : base(titleViewModel)
        {
            _optionsView = optionsView;
            _customGameSeedView = customGameSeedView;
            _loadGameView = loadGameView;
            _loadReplayView = loadReplayView;
            _developerToolsView = developerToolsView;
        }

        protected override void InitializeInternal()
        {
            _titleMenuPanel = new Panel()
                .AutoHeight()
                .WidthOfButtonWithPadding()
                .OpacityPercent(90);

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

            SetupChildPanelWithButton<OpenLoadReplayViewRequest, LoadReplayViewModel, LoadReplayData>(
                _titleMenuPanel,
                "Open Recording",
                _loadReplayView);

            SetupChildPanelWithButton<OptionsButtonClickedRequest, OptionsViewModel, OptionsData>(
                _titleMenuPanel,
                "Options",
                _optionsView);

            new Button("Exit")
                .SendOnClick<QuitToDesktopRequest>(Mediator)
                .AddTo(_titleMenuPanel);

            SetupChildPanelWithButton<DeveloperToolsButtonClickedRequest, DeveloperToolsViewModel, DeveloperToolsData>(
                _titleMenuPanel,
                "Developer Tools",
                _developerToolsView);

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

        public Task<Unit> Handle(DeveloperToolsButtonClickedRequest request, CancellationToken cancellationToken)
        {
            return ShowChildView(_developerToolsView, _titleMenuPanel);
        }

        public Task<Unit> Handle(CloseDeveloperToolsViewRequest request, CancellationToken cancellationToken)
        {
            return HideChildView(_developerToolsView, _titleMenuPanel);
        }

        public Task<Unit> Handle(OpenLoadGameViewRequest request, CancellationToken cancellationToken)
        {
            return ShowChildView(_loadGameView, _titleMenuPanel);
        }

        public Task<Unit> Handle(CloseLoadGameViewRequest request, CancellationToken cancellationToken)
        {
            return HideChildView(_loadGameView, _titleMenuPanel);
        }

        public Task<Unit> Handle(OpenLoadReplayViewRequest request, CancellationToken cancellationToken)
        {
            return ShowChildView(_loadReplayView, _titleMenuPanel);
        }

        public Task<Unit> Handle(CloseLoadReplayViewRequest request, CancellationToken cancellationToken)
        {
            return HideChildView(_loadReplayView, _titleMenuPanel);
        }

        public override void Draw()
        {
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            _spriteBatch.Draw(Assets.TitleTexture, Game.GraphicsDevice.ViewportRectangle(), Color.White);
            _spriteBatch.Draw(
                Assets.TitleTextTexture,
                new Vector2(
                    Game.GraphicsDevice.ViewportRectangle().Width / 2 - Assets.TitleTextTexture.Width / 2,
                    Game.GraphicsDevice.ViewportRectangle().Height / 10f
                    ),
                Color.White);
            _spriteBatch.End();
        }
    }
}