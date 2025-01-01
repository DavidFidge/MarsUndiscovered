using System.Threading;
using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.MonoGame.Core.Messages;
using FrigidRogue.MonoGame.Core.View.Extensions;
using GeonBit.UI.Entities;
using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;
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
        IRequestHandler<CloseLoadGameViewRequest>
    {
        private readonly OptionsView _optionsView;
        private readonly CustomGameSeedView _customGameSeedView;
        private readonly LoadGameView _loadGameView;
        private readonly DeveloperToolsView _developerToolsView;

        private Panel _titleMenuPanel;

        private SpriteBatch _spriteBatch;

        public TitleView(
            TitleViewModel titleViewModel,
            OptionsView optionsView,
            CustomGameSeedView customGameSeedView,
            LoadGameView loadGameView,
            DeveloperToolsView developerToolsView
            )
            : base(titleViewModel)
        {
            _optionsView = optionsView;
            _customGameSeedView = customGameSeedView;
            _loadGameView = loadGameView;
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

        public void Handle(CustomGameSeedRequest request)
        {
            ShowChildView(_customGameSeedView, _titleMenuPanel);
        }

        public void Handle(CancelCustomGameSeedRequest request)
        {
            HideChildView(_customGameSeedView, _titleMenuPanel);
        }

        public void Handle(OptionsButtonClickedRequest request)
        {
            ShowChildView(_optionsView, _titleMenuPanel);
        }

        public void Handle(CloseOptionsViewRequest request)
        {
            HideChildView(_optionsView, _titleMenuPanel);
        }

        public void Handle(DeveloperToolsButtonClickedRequest request)
        {
            ShowChildView(_developerToolsView, _titleMenuPanel);
        }

        public void Handle(CloseDeveloperToolsViewRequest request)
        {
            HideChildView(_developerToolsView, _titleMenuPanel);
        }

        public void Handle(OpenLoadGameViewRequest request)
        {
            ShowChildView(_loadGameView, _titleMenuPanel);
        }

        public void Handle(CloseLoadGameViewRequest request)
        {
            HideChildView(_loadGameView, _titleMenuPanel);
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