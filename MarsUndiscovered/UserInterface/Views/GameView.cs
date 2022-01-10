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
        IRequestHandler<MouseHoverViewRequest>
    {
        private readonly InGameOptionsView _inGameOptionsView;
        private readonly ConsoleView _consoleView;
        private readonly GameViewGameOverKeyboardHandler _gameOverKeyboardHandler;

        public GameView(
            GameViewModel gameViewModel,
            InGameOptionsView inGameOptionsView,
            ConsoleView consoleView,
            GameViewGameOverKeyboardHandler gameOverKeyboardHandler,
            IGameCamera gameCamera
        )
            : base(gameCamera, gameViewModel)
        {
            _inGameOptionsView = inGameOptionsView;
            _consoleView = consoleView;
            _gameOverKeyboardHandler = gameOverKeyboardHandler;
        }

        protected override void InitializeInternal()
        {
            SetupChildPanel(_inGameOptionsView);
            SetupConsole();
            CreateLayoutPanels();
            SetupInGameOptionsButton(LeftPanel);
            CreatePlayerPanel();
            CreateMessageLog();
            CreateStatusPanel();
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

        public Task<Unit> Handle(LeftClickViewRequest request, CancellationToken cancellationToken)
        {
            var ray = _gameCamera.GetPointerRay(request.X, request.Y);
            _viewModel.Move(ray);

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

        public Task<Unit> Handle(MoveWaitRequest request, CancellationToken cancellationToken)
        {
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

        public Task<Unit> Handle(MouseHoverViewRequest request, CancellationToken cancellationToken)
        {
            var ray = _gameCamera.GetPointerRay(request.X, request.Y);

            _viewModel.MapViewModel.ShowHover(ray);

            return Unit.Task;
        }
    }
}