using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FrigidRogue.MonoGame.Core.Graphics.Camera;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.View.Extensions;

using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

using GeonBit.UI.Entities;

using GoRogue.Pathing;

using MarsUndiscovered.UserInterface.Input;

using MediatR;

using Microsoft.Xna.Framework;

namespace MarsUndiscovered.UserInterface.Views
{
    public class GameView : BaseGameView<GameViewModel, GameData>,
        IRequestHandler<OpenInGameOptionsRequest>,
        IRequestHandler<CloseInGameOptionsRequest>,
        IRequestHandler<OpenConsoleRequest>,
        IRequestHandler<CloseConsoleRequest>,
        IRequestHandler<OpenGameInventoryRequest>,
        IRequestHandler<CloseGameInventoryRequest>,
        IRequestHandler<LeftClickViewRequest>,
        IRequestHandler<RightClickViewRequest>,
        IRequestHandler<MoveUpRequest>,
        IRequestHandler<MoveUpLeftRequest>,
        IRequestHandler<MoveUpRightRequest>,
        IRequestHandler<MoveDownRequest>,
        IRequestHandler<MoveDownLeftRequest>,
        IRequestHandler<MoveDownRightRequest>,
        IRequestHandler<MoveLeftRequest>,
        IRequestHandler<MoveRightRequest>,
        IRequestHandler<MoveWaitRequest>
    {
        private readonly InGameOptionsView _inGameOptionsView;
        private readonly ConsoleView _consoleView;
        private readonly InventoryGameView _inventoryGameView;
        private readonly GameViewGameOverKeyboardHandler _gameOverKeyboardHandler;
        private readonly IStopwatchProvider _stopwatchProvider;
        private Path _currentMovePath;
        private double _lastMovePathTime = 0;
        private double _delayBetweenMovePath = 50;

        public GameView(
            GameViewModel gameViewModel,
            InGameOptionsView inGameOptionsView,
            ConsoleView consoleView,
            InventoryGameView inventoryGameView,
            GameViewGameOverKeyboardHandler gameOverKeyboardHandler,
            IGameCamera gameCamera,
            IStopwatchProvider stopwatchProvider
        )
            : base(gameCamera, gameViewModel)
        {
            _inGameOptionsView = inGameOptionsView;
            _consoleView = consoleView;
            _inventoryGameView = inventoryGameView;
            _gameOverKeyboardHandler = gameOverKeyboardHandler;
            _stopwatchProvider = stopwatchProvider;
        }

        protected override void InitializeInternal()
        {
            CreateLayoutPanels();
            SetupInGameOptionsButton(LeftPanel);
            CreatePlayerPanel();
            CreateMessageLog();
            CreateStatusPanel();
            SetupConsole();
            SetupInventoryGame();
            SetupChildPanel(_inGameOptionsView);

            _stopwatchProvider.Start();
        }

        public void NewGame(uint? seed = null)
        {
            ResetViews();
            _viewModel.NewGame(seed);
        }

        public void LoadGame(string filename)
        {
            ResetViews();
            _viewModel.LoadGame(filename);
        }

        private void SetupInGameOptionsButton(Panel leftPanel)
        {
            var menuButton = new Button(
                    "-",
                    ButtonSkin.Default,
                    Anchor.Auto,
                    new Vector2(50, 50)
                )
                .SendOnClick<OpenInGameOptionsRequest>(Mediator)
                .NoPadding();

            leftPanel.AddChild(menuButton);
        }

        private void SetupConsole()
        {
            _consoleView.Initialize();

            RootPanel.AddChild(_consoleView.RootPanel);
        }

        private void SetupInventoryGame()
        {
            _inventoryGameView.Initialize();

            RootPanel.AddChild(_inventoryGameView.RootPanel);
        }

        private void BeforeViewChange()
        {
            _currentMovePath = null;
        }

        public Task<Unit> Handle(OpenInGameOptionsRequest request, CancellationToken cancellationToken)
        {
            BeforeViewChange();
            _inGameOptionsView.Show();
            return Unit.Task;
        }

        public Task<Unit> Handle(CloseInGameOptionsRequest request, CancellationToken cancellationToken)
        {
            BeforeViewChange();
            _inGameOptionsView.Hide();
            return Unit.Task;
        }

        public Task<Unit> Handle(OpenConsoleRequest request, CancellationToken cancellationToken)
        {
            BeforeViewChange();
            _consoleView.Show();
            return Unit.Task;
        }

        public Task<Unit> Handle(CloseConsoleRequest request, CancellationToken cancellationToken)
        {
            _consoleView.Hide();
            return Unit.Task;
        }

        public Task<Unit> Handle(LeftClickViewRequest request, CancellationToken cancellationToken)
        {
            BeforeViewChange();
            var ray = _gameCamera.GetPointerRay(request.X, request.Y);
            _currentMovePath = _viewModel.GetPathToDestination(ray);

            return Unit.Task;
        }

        public Task<Unit> Handle(RightClickViewRequest request, CancellationToken cancellationToken)
        {
            BeforeViewChange();
            return Unit.Task;
        }

        public Task<Unit> Handle(MoveUpRequest request, CancellationToken cancellationToken)
        {
            BeforeViewChange();
            _viewModel.Move(request.Direction);

            return Unit.Task;
        }

        public Task<Unit> Handle(MoveDownRequest request, CancellationToken cancellationToken)
        {
            BeforeViewChange();
            _viewModel.Move(request.Direction);

            return Unit.Task;
        }

        public Task<Unit> Handle(MoveLeftRequest request, CancellationToken cancellationToken)
        {
            BeforeViewChange();
            _viewModel.Move(request.Direction);

            return Unit.Task;
        }

        public Task<Unit> Handle(MoveRightRequest request, CancellationToken cancellationToken)
        {
            BeforeViewChange();
            _viewModel.Move(request.Direction);

            return Unit.Task;
        }

        public Task<Unit> Handle(MoveUpLeftRequest request, CancellationToken cancellationToken)
        {
            BeforeViewChange();
            _viewModel.Move(request.Direction);

            return Unit.Task;
        }

        public Task<Unit> Handle(MoveUpRightRequest request, CancellationToken cancellationToken)
        {
            BeforeViewChange();
            _viewModel.Move(request.Direction);

            return Unit.Task;
        }

        public Task<Unit> Handle(MoveDownLeftRequest request, CancellationToken cancellationToken)
        {
            BeforeViewChange();
            _viewModel.Move(request.Direction);

            return Unit.Task;
        }

        public Task<Unit> Handle(MoveDownRightRequest request, CancellationToken cancellationToken)
        {
            BeforeViewChange();
            _viewModel.Move(request.Direction);

            return Unit.Task;
        }

        public Task<Unit> Handle(MoveWaitRequest request, CancellationToken cancellationToken)
        {
            BeforeViewChange();
            _viewModel.Move(request.Direction);

            return Unit.Task;
        }

        protected override void ViewModelChanged()
        {
            base.ViewModelChanged();

            if (_viewModel.PlayerStatus.IsDead)
            {
                GameInputService?.ChangeInput(MouseHandler, _gameOverKeyboardHandler);
                StatusParagraph.Text = DelimitWithDashes("YOU ARE DEAD. PRESS SPACE TO EXIT GAME.");
            }
        }

        public override Task Handle(MouseHoverViewNotification notification, CancellationToken cancellationToken)
        {
            base.Handle(notification, cancellationToken);

            var ray = _gameCamera.GetPointerRay(notification.X, notification.Y);

            _viewModel.MapViewModel.ShowHover(ray);

            return Unit.Task;
        }

        public override void Update()
        {
            base.Update();

            if (_currentMovePath == null)
                return;

            if (_stopwatchProvider.Elapsed.TotalMilliseconds - _lastMovePathTime > _delayBetweenMovePath)
            {
                _lastMovePathTime = _stopwatchProvider.Elapsed.TotalMilliseconds;
                var isComplete = _viewModel.Move(_currentMovePath);

                if (isComplete)
                    _currentMovePath = null;
            }
        }

        public Task<Unit> Handle(OpenGameInventoryRequest request, CancellationToken cancellationToken)
        {
            BeforeViewChange();
            _inventoryGameView.Show();
            return Unit.Task;
        }

        public Task<Unit> Handle(CloseGameInventoryRequest request, CancellationToken cancellationToken)
        {
            _inventoryGameView.Hide();
            return Unit.Task;
        }
    }
}