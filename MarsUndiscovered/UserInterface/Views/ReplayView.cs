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
        IRequestHandler<CloseInReplayOptionsRequest>,
        IRequestHandler<OpenReplayInventoryRequest>,
        IRequestHandler<CloseReplayInventoryRequest>
    {
        private readonly InReplayOptionsView _inReplayOptionsView;
        private readonly InventoryReplayView _inventoryReplayView;
        private Label _turnLabel;

        public ReplayView(
            ReplayViewModel replayViewModel,
            InReplayOptionsView inReplayOptionsView,
            InventoryReplayView inventoryReplayView,
            IGameCamera gameCamera
        )
            : base(gameCamera, replayViewModel)
        {
            _inReplayOptionsView = inReplayOptionsView;
            _inventoryReplayView = inventoryReplayView;
        }

        protected override void InitializeInternal()
        {
            CreateLayoutPanels();
            SetupInReplayOptionsButton(LeftPanel);
            CreateReplayView();
            CreatePlayerPanel();
            CreateMessageLog();
            CreateStatusPanel();
            SetupInventoryGame();
            SetupChildPanel(_inReplayOptionsView);
        }

        private void SetupInventoryGame()
        {
            _inventoryReplayView.Initialize();

            RootPanel.AddChild(_inventoryReplayView.RootPanel);
        }

        private void CreateReplayView()
        {
            var replayLabel = new Label("RECORDING")
                .Anchor(Anchor.AutoInline)
                .NoPadding();

            LeftPanel.AddChild(replayLabel);

            _turnLabel = new Label("Turn 1")
                .Anchor(Anchor.AutoInline)
                .NoPadding();

            LeftPanel.AddChild(_turnLabel);
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
            var nextReplayResult = _viewModel.ExecuteNextReplayCommand();

            if (nextReplayResult == false)
                StatusParagraph.Text = DelimitWithDashes("END OF RECORDING");

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
            ResetViews();
            _viewModel.LoadReplay(filename);
        }

        protected override void ViewModelChanged()
        {
            base.ViewModelChanged();

            _turnLabel.Text = $"Turn {_viewModel.TurnNumber}";
        }

        public Task<Unit> Handle(OpenReplayInventoryRequest request, CancellationToken cancellationToken)
        {
            _inventoryReplayView.Show();
            return Unit.Task;
        }

        public Task<Unit> Handle(CloseReplayInventoryRequest request, CancellationToken cancellationToken)
        {
            _inventoryReplayView.Hide();
            return Unit.Task;
        }
    }
}