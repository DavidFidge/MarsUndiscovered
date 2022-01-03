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
using Microsoft.Xna.Framework.Graphics;

namespace MarsUndiscovered.UserInterface.Views
{
    public class ReplayView : BaseMarsUndiscoveredView<ReplayViewModel, ReplayData>,
        IRequestHandler<NextReplayCommandRequest>,
        IRequestHandler<OpenInReplayOptionsRequest>,
        IRequestHandler<CloseInReplayOptionsRequest>
    {
        public bool IsMouseInGameView => RootPanel?.IsMouseInRootPanelEmptySpace ?? true;

        private readonly InReplayOptionsView _inReplayOptionsView;
        private readonly IGameCamera _gameCamera;
        private SelectList _messageLog;
        private Label _turnLabel;
        private int _turnNumber = 1;

        public ReplayView(
            ReplayViewModel gameViewModel,
            InReplayOptionsView inReplayOptionsView,
            IGameCamera gameCamera
        )
            : base(gameViewModel)
        {
            _inReplayOptionsView = inReplayOptionsView;
            _gameCamera = gameCamera;
        }

        protected override void InitializeInternal()
        {
            SetupInReplayOptions();
        }

        private void SetupInReplayOptions()
        {
            var leftPanel = new Panel()
                .WithAnchor(Anchor.TopLeft)
                .NoSkin()
                .NoPadding()
                .AutoHeight();

            RootPanel.AddChild(leftPanel);

            var menuButton = new Button(
                "-",
                ButtonSkin.Default,
                Anchor.AutoInline,
                new Vector2(50, 50))
                .SendOnClick<OpenInReplayOptionsRequest>(Mediator)
                .NoPadding();

            leftPanel.AddChild(menuButton);

            var replayLabel = new Label("REPLAY")
                .WithAnchor(Anchor.AutoInline)
                .NoPadding();

            leftPanel.AddChild(replayLabel);

            _turnLabel = new Label("Turn 1")
                .WithAnchor(Anchor.AutoInline)
                .NoPadding();

            leftPanel.AddChild(_turnLabel);

            SetupChildPanel(_inReplayOptionsView);

            _messageLog = new SelectList(new Vector2(0.61f, 0.14f), Anchor.TopCenter, null, PanelSkin.None)
                .NoPadding();

            _messageLog.ExtraSpaceBetweenLines = -10;
            _messageLog.LockSelection = true;
            RootPanel.AddChild(_messageLog);

            _messageLog.OnListChange = entity =>
            {
                var list = (SelectList)entity;
                if (list.Count > 100)
                    list.RemoveItem(0);
            };
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

        public override void Draw()
        {
            var oldDepthStencilState = Game.GraphicsDevice.DepthStencilState;
            Game.GraphicsDevice.DepthStencilState = DepthStencilState.None;

            _viewModel.SceneGraph.Draw(_gameCamera.View, _gameCamera.Projection);

            Game.GraphicsDevice.DepthStencilState = oldDepthStencilState;

            base.Draw();
        }

        public override void Update()
        {
            var newMessages = _viewModel.GetNewMessages();

            if (newMessages.Any())
            {
                foreach (var message in newMessages)
                    _messageLog.AddItem(message);

                _messageLog.scrollToEnd();
            }

            _gameCamera.Update();

            base.Update();
        }

        public void LoadReplay(string filename)
        {
            _viewModel.LoadReplay(filename);
            ResetViews();
        }

        private void ResetViews()
        {
            _messageLog.ClearItems();
            _turnLabel.Text = "Turn 1";
            _turnNumber = 1;
        }
    }
}