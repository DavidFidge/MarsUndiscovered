using System;
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
        IRequestHandler<CloseOptionsViewRequest>,
        IRequestHandler<CustomGameSeedRequest>,
        IRequestHandler<CancelCustomGameSeedRequest>
    {
        private readonly OptionsView _optionsView;
        private readonly CustomGameSeedView _customGameSeedView;

        private Panel _titleMenuPanel;

        private SpriteBatch _spriteBatch;

        public TitleView(
            TitleViewModel titleViewModel,
            OptionsView optionsView,
            CustomGameSeedView customGameSeedView)
            : base(titleViewModel)
        {
            _optionsView = optionsView;
            _customGameSeedView = customGameSeedView;
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


            SetupCustomGameItem();
            SetupOptionsItem();

            new Button("Exit")
                .SendOnClick<QuitToDesktopRequest>(Mediator)
                .AddTo(_titleMenuPanel);

            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
        }

        private void SetupOptionsItem()
        {
            new Button("Options")
                .SendOnClick<OptionsButtonClickedRequest>(Mediator)
                .AddTo(_titleMenuPanel);

            _optionsView.Initialize();

            RootPanel.AddChild(_optionsView.RootPanel);
        }

        public Task<Unit> Handle(OptionsButtonClickedRequest request, CancellationToken cancellationToken)
        {
            _optionsView.Show();
            _titleMenuPanel.Visible = false;

            return Unit.Task;
        }

        private void SetupCustomGameItem()
        {
            new Button("New Game (custom seed)")
                .SendOnClick<CustomGameSeedRequest>(Mediator)
                .AddTo(_titleMenuPanel);

            _customGameSeedView.Initialize();

            RootPanel.AddChild(_customGameSeedView.RootPanel);
        }

        public Task<Unit> Handle(CustomGameSeedRequest request, CancellationToken cancellationToken)
        {
            _customGameSeedView.Show();
            _titleMenuPanel.Visible = false;

            return Unit.Task;
        }

        public Task<Unit> Handle(CancelCustomGameSeedRequest request, CancellationToken cancellationToken)
        {
            _customGameSeedView.Hide();
            _titleMenuPanel.Visible = true;

            return Unit.Task;
        }

        public Task<Unit> Handle(CloseOptionsViewRequest request, CancellationToken cancellationToken)
        {
            _optionsView.Hide();
            _titleMenuPanel.Visible = true;

            return Unit.Task;
        }

        public override void Draw()
        {
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            _spriteBatch.Draw(Assets.TitleTexture, Game.GraphicsDevice.ViewportRectangle(), Color.White);
            _spriteBatch.End();
        }
    }
}