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
        IRequestHandler<MoveWaitRequest>,
        IRequestHandler<AutoExploreRequest>,
        IRequestHandler<EndModalRequest>
    {
        private readonly InGameOptionsView _inGameOptionsView;
        private readonly ConsoleView _consoleView;
        private readonly InventoryGameView _inventoryGameView;
        private readonly GameViewGameOverKeyboardHandler _gameOverKeyboardHandler;
        private readonly GameViewModalKeyboardHandler _modalKeyboardHandler;
        private readonly IStopwatchProvider _stopwatchProvider;
        private Path _currentMovePath;
        private bool _isAutoExploring;
        private double _lastMoveTime = 0;
        private double _delayBetweenMove = 50;

        public GameView(
            GameViewModel gameViewModel,
            InGameOptionsView inGameOptionsView,
            ConsoleView consoleView,
            InventoryGameView inventoryGameView,
            GameViewGameOverKeyboardHandler gameOverKeyboardHandler,
            GameViewModalKeyboardHandler modalKeyboardHandler,
            IGameCamera gameCamera,
            IStopwatchProvider stopwatchProvider
        )
            : base(gameCamera, gameViewModel)
        {
            _inGameOptionsView = inGameOptionsView;
            _consoleView = consoleView;
            _inventoryGameView = inventoryGameView;
            _gameOverKeyboardHandler = gameOverKeyboardHandler;
            _modalKeyboardHandler = modalKeyboardHandler;
            _stopwatchProvider = stopwatchProvider;
        }

        protected override void InitializeInternal()
        {
            base.InitializeInternal();
            
            CreateLayoutPanels();
            SetupInGameOptionsButton(LeftPanel);
            CreatePlayerPanel();
            CreateMessageLog();
            CreateStatusPanel();
            CreateRadioCommsPanel();
            SetupConsole();
            SetupInventoryGame();
            SetupChildPanel(_inGameOptionsView);

            _stopwatchProvider.Start();
        }

        public void NewGame(ulong? seed = null)
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

        private void StopAutoMovement()
        {
            _currentMovePath = null;
            _isAutoExploring = false;
        }

        public Task<Unit> Handle(OpenInGameOptionsRequest request, CancellationToken cancellationToken)
        {
            StopAutoMovement();
            _inGameOptionsView.Show();
            return Unit.Task;
        }

        public Task<Unit> Handle(CloseInGameOptionsRequest request, CancellationToken cancellationToken)
        {
            StopAutoMovement();
            _inGameOptionsView.Hide();
            return Unit.Task;
        }

        public Task<Unit> Handle(OpenConsoleRequest request, CancellationToken cancellationToken)
        {
            StopAutoMovement();
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
            StopAutoMovement();
            var ray = _gameCamera.GetPointerRay(request.X, request.Y);
            _currentMovePath = _viewModel.GetPathToDestination(ray);

            return Unit.Task;
        }

        public Task<Unit> Handle(RightClickViewRequest request, CancellationToken cancellationToken)
        {
            StopAutoMovement();
            return Unit.Task;
        }

        public Task<Unit> Handle(MoveUpRequest request, CancellationToken cancellationToken)
        {
            StopAutoMovement();
            _viewModel.Move(request.Direction);

            return Unit.Task;
        }

        public Task<Unit> Handle(MoveDownRequest request, CancellationToken cancellationToken)
        {
            StopAutoMovement();
            _viewModel.Move(request.Direction);

            return Unit.Task;
        }

        public Task<Unit> Handle(MoveLeftRequest request, CancellationToken cancellationToken)
        {
            StopAutoMovement();
            _viewModel.Move(request.Direction);

            return Unit.Task;
        }

        public Task<Unit> Handle(MoveRightRequest request, CancellationToken cancellationToken)
        {
            StopAutoMovement();
            _viewModel.Move(request.Direction);

            return Unit.Task;
        }

        public Task<Unit> Handle(MoveUpLeftRequest request, CancellationToken cancellationToken)
        {
            StopAutoMovement();
            _viewModel.Move(request.Direction);

            return Unit.Task;
        }

        public Task<Unit> Handle(MoveUpRightRequest request, CancellationToken cancellationToken)
        {
            StopAutoMovement();
            _viewModel.Move(request.Direction);

            return Unit.Task;
        }

        public Task<Unit> Handle(MoveDownLeftRequest request, CancellationToken cancellationToken)
        {
            StopAutoMovement();
            _viewModel.Move(request.Direction);

            return Unit.Task;
        }

        public Task<Unit> Handle(MoveDownRightRequest request, CancellationToken cancellationToken)
        {
            StopAutoMovement();
            _viewModel.Move(request.Direction);

            return Unit.Task;
        }

        public Task<Unit> Handle(MoveWaitRequest request, CancellationToken cancellationToken)
        {
            StopAutoMovement();
            _viewModel.Move(request.Direction);

            return Unit.Task;
        }

        protected override void ViewModelChanged()
        {
            base.ViewModelChanged();

            if (_viewModel.PlayerStatus.IsDead || _viewModel.PlayerStatus.IsVictorious)
            {
                _viewModel.WriteAndSendMorgue();

                if (_viewModel.PlayerStatus.IsDead)
                {
                    GameInputService?.ChangeInput(MouseHandler, _gameOverKeyboardHandler);
                    StatusParagraph.Text = DelimitWithDashes("YOU ARE DEAD. PRESS SPACE TO EXIT GAME.");
                }
                else if (_viewModel.PlayerStatus.IsVictorious)
                {
                    GameInputService?.ChangeInput(MouseHandler, _gameOverKeyboardHandler);
                    StatusParagraph.Text = DelimitWithDashes("YOU ARE VICTORIOUS! PRESS SPACE TO EXIT GAME.");
                }
            }
        }

        public override Task Handle(MouseHoverViewNotification notification, CancellationToken cancellationToken)
        {
            if (!IsVisible)
                return Unit.Task;
            
            base.Handle(notification, cancellationToken);

            var ray = _gameCamera.GetPointerRay(notification.X, notification.Y);

            _viewModel.MapViewModel.ShowHover(ray);

            return Unit.Task;
        }

        public override void Update()
        {
            base.Update();

            _viewModel.UpdateAnimation();

            if (_viewModel.IsAnimating)
                return;

            if (_currentMovePath != null)
            {
                if (_stopwatchProvider.Elapsed.TotalMilliseconds - _lastMoveTime > _delayBetweenMove)
                {
                    _lastMoveTime = _stopwatchProvider.Elapsed.TotalMilliseconds;
                    var isComplete = _viewModel.Move(_currentMovePath);

                    if (isComplete)
                        _currentMovePath = null;
                }
            }
            else if (_isAutoExploring)
            {
                if (_stopwatchProvider.Elapsed.TotalMilliseconds - _lastMoveTime > _delayBetweenMove)
                {
                    _lastMoveTime = _stopwatchProvider.Elapsed.TotalMilliseconds;
                    var autoExploreResult = _viewModel.AutoExplore();

                    if (autoExploreResult.MovementInterrupted || autoExploreResult.Path.Length == 0)
                        _isAutoExploring = false;
                }
            }
        }

        public Task<Unit> Handle(OpenGameInventoryRequest request, CancellationToken cancellationToken)
        {
            if (!IsVisible)
                return Unit.Task;
            
            StopAutoMovement();
            _inventoryGameView.SetInventoryMode(request.InventoryMode);
            _inventoryGameView.Show();
            return Unit.Task;
        }

        public Task<Unit> Handle(CloseGameInventoryRequest request, CancellationToken cancellationToken)
        {
            if (!IsVisible)
                return Unit.Task;
            
            _inventoryGameView.Hide();
            return Unit.Task;
        }

        public Task<Unit> Handle(AutoExploreRequest request, CancellationToken cancellationToken)
        {
            if (!IsVisible)
                return Unit.Task;
            
            _isAutoExploring = true;
            return Unit.Task;
        }

        protected override void ResetViews()
        {
            base.ResetViews();
            _isAutoExploring = false;
            _currentMovePath = null;
        }

        public Task<Unit> Handle(EndModalRequest request, CancellationToken cancellationToken)
        {
            
        }
    }
}