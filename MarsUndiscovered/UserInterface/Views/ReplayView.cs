using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FrigidRogue.MonoGame.Core.Graphics.Camera;
using FrigidRogue.MonoGame.Core.View.Extensions;

using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

using GeonBit.UI.Entities;

using MediatR;

using Microsoft.Xna.Framework;

namespace MarsUndiscovered.UserInterface.Views
{
    public class ReplayView : BaseGameView<ReplayViewModel, ReplayData>,
        IRequestHandler<NextReplayCommandRequest>,
        IRequestHandler<OpenInReplayOptionsRequest>,
        IRequestHandler<CloseInReplayOptionsRequest>
    {
        private readonly InReplayOptionsView _inReplayOptionsView;
        private Label _turnLabel;
        private int _turnNumber = 1;

        public ReplayView(
            ReplayViewModel gameViewModel,
            InReplayOptionsView inReplayOptionsView,
            IGameCamera gameCamera
        )
            : base(gameCamera, gameViewModel)
        {
            _inReplayOptionsView = inReplayOptionsView;
        }

        protected override void InitializeInternal()
        {
            var leftPanel = new Panel()
                .WithAnchor(Anchor.TopLeft)
                .NoSkin()
                .NoPadding()
                .AutoHeight();

            RootPanel.AddChild(leftPanel);

            SetupInReplayOptionsButton(leftPanel);

            var replayLabel = new Label("REPLAY")
                .WithAnchor(Anchor.AutoInline)
                .NoPadding();

            leftPanel.AddChild(replayLabel);

            _turnLabel = new Label("Turn 1")
                .WithAnchor(Anchor.AutoInline)
                .NoPadding();

            leftPanel.AddChild(_turnLabel);

            AddMessageLog();

            SetupChildPanel(_inReplayOptionsView);
        }

        private void SetupInReplayOptionsButton(Panel leftPanel)
        {
            var menuButton = new Button(
                    "-",
                    ButtonSkin.Default,
                    Anchor.AutoInline,
                    new Vector2(50, 50)
                )
                .SendOnClick<OpenInReplayOptionsRequest>(Mediator)
                .NoPadding();

            leftPanel.AddChild(menuButton);
        }

        public Task<Unit> Handle(NextReplayCommandRequest request, CancellationToken cancellationToken)
        {
            _viewModel.ExecuteNextReplayCommand();

            _turnLabel.Text = $"Turn {++_turnNumber}";

            return Unit.Task;
        }

        public Task<Unit> Handle(OpenInReplayOptionsRequest request, CancellationToken cancellationToken)
        {
            _inReplayOptionsView.Show();
            return Unit.Task;
        }

        public Task<Unit> Handle(CloseInReplayOptionsRequest request, CancellationToken cancellationToken)
        {
            _inReplayOptionsView.Hide();
            return Unit.Task;
        }

        public void LoadReplay(string filename)
        {
            _viewModel.LoadReplay(filename);
            ResetViews();
        }

        protected override void ResetViews()
        {
            base.ResetViews();
            _turnLabel.Text = "Turn 1";
            _turnNumber = 1;
        }
    }
}