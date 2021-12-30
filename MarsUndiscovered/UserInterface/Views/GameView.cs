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
    public class GameView : BaseMarsUndiscoveredView<GameViewModel, GameData>,
        IRequestHandler<OpenInGameOptionsRequest>,
        IRequestHandler<CloseInGameOptionsRequest>,
        IRequestHandler<OpenConsoleRequest>,
        IRequestHandler<CloseConsoleRequest>,
        IRequestHandler<LeftClickViewRequest>,
        IRequestHandler<RightClickViewRequest>,
        IRequestHandler<MoveUpRequest>,
        IRequestHandler<MoveUpLeftRequest>,
        IRequestHandler<MoveUpRightRequest>,
        IRequestHandler<MoveDownRequest>,
        IRequestHandler<MoveDownLeftRequest>,
        IRequestHandler<MoveDownRightRequest>,
        IRequestHandler<MoveLeftRequest>,
        IRequestHandler<MoveRightRequest>
    {
        public bool IsMouseInGameView => RootPanel?.IsMouseInRootPanelEmptySpace ?? true;

        private readonly InGameOptionsView _inGameOptionsView;
        private readonly ConsoleView _consoleView;
        private readonly IGameCamera _gameCamera;
        private SelectList _messageLog;

        public GameView(
            GameViewModel gameViewModel,
            InGameOptionsView inGameOptionsView,
            ConsoleView consoleView,
            IGameCamera gameCamera
        )
            : base(gameViewModel)
        {
            _inGameOptionsView = inGameOptionsView;
            _consoleView = consoleView;
            _gameCamera = gameCamera;
            _gameCamera.MoveSensitivity = 1f;
            _gameCamera.RotateSensitivity = 0.01f;
            _gameCamera.ZoomSensitivity = 0.01f;
        }

        protected override void InitializeInternal()
        {
            SetupInGameOptions();
            SetupConsole();

            _gameCamera.Initialise();
        }

        private void SetupInGameOptions()
        {
            var menuButton = new Button(
                "-",
                ButtonSkin.Default,
                Anchor.TopLeft,
                new Vector2(50, 50))
                .SendOnClick<OpenInGameOptionsRequest>(Mediator)
                .NoPadding();


            RootPanel.AddChild(menuButton);

            SetupChildPanel(_inGameOptionsView);

            _messageLog = new SelectList(new Vector2(0.68f, 0.14f), Anchor.TopCenter, null, PanelSkin.None)
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

        private void SetupConsole()
        {
            _consoleView.Initialize();

            RootPanel.AddChild(_consoleView.RootPanel);
        }

        public Task<Unit> Handle(OpenInGameOptionsRequest request, CancellationToken cancellationToken)
        {
            _inGameOptionsView.Show();
            return Unit.Task;
        }

        public Task<Unit> Handle(CloseInGameOptionsRequest request, CancellationToken cancellationToken)
        {
            _inGameOptionsView.Hide();
            return Unit.Task;
        }

        public Task<Unit> Handle(OpenConsoleRequest request, CancellationToken cancellationToken)
        {
            _consoleView.Show();
            return Unit.Task;
        }

        public Task<Unit> Handle(CloseConsoleRequest request, CancellationToken cancellationToken)
        {
            _consoleView.Hide();
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

        public void NewGame(uint? seed = null)
        {
            _viewModel.NewGame(seed);
            ResetViews();
        }

        public void LoadGame(string filename)
        {
            _viewModel.LoadGame(filename);
            ResetViews();
        }

        private void ResetViews()
        {
            _messageLog.ClearItems();
        }

        public Task<Unit> Handle(LeftClickViewRequest request, CancellationToken cancellationToken)
        {
            return Unit.Task;
        }

        public Task<Unit> Handle(RightClickViewRequest request, CancellationToken cancellationToken)
        {
            return Unit.Task;
        }

        public Task<Unit> Handle(MoveUpRequest request, CancellationToken cancellationToken)
        {
            _viewModel.Move(request.Direction);

            return Unit.Task;
        }

        public Task<Unit> Handle(MoveDownRequest request, CancellationToken cancellationToken)
        {
            _viewModel.Move(request.Direction);

            return Unit.Task;
        }

        public Task<Unit> Handle(MoveLeftRequest request, CancellationToken cancellationToken)
        {
            _viewModel.Move(request.Direction);

            return Unit.Task;
        }

        public Task<Unit> Handle(MoveRightRequest request, CancellationToken cancellationToken)
        {
            _viewModel.Move(request.Direction);

            return Unit.Task;
        }

        public Task<Unit> Handle(MoveUpLeftRequest request, CancellationToken cancellationToken)
        {
            _viewModel.Move(request.Direction);

            return Unit.Task;
        }

        public Task<Unit> Handle(MoveUpRightRequest request, CancellationToken cancellationToken)
        {
            _viewModel.Move(request.Direction);

            return Unit.Task;
        }

        public Task<Unit> Handle(MoveDownLeftRequest request, CancellationToken cancellationToken)
        {
            _viewModel.Move(request.Direction);

            return Unit.Task;
        }

        public Task<Unit> Handle(MoveDownRightRequest request, CancellationToken cancellationToken)
        {
            _viewModel.Move(request.Direction);

            return Unit.Task;
        }
    }
}