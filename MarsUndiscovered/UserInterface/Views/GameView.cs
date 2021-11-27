using System.Threading;
using System.Threading.Tasks;

using Augmented.Messages;
using Augmented.UserInterface.Data;
using Augmented.UserInterface.ViewModels;

using DavidFidge.Monogame.Core.View;
using DavidFidge.Monogame.Core.View.Extensions;

using GeonBit.UI.Entities;

using MediatR;

using Microsoft.Xna.Framework;

namespace Augmented.UserInterface.Views
{
    public class GameView : BaseView<GameViewModel, GameData>,
        IRequestHandler<OpenInGameOptionsRequest>,
        IRequestHandler<CloseInGameOptionsRequest>,
        IRequestHandler<OpenConsoleRequest>,
        IRequestHandler<CloseConsoleRequest>
    {
        private readonly InGameOptionsView _inGameOptionsView;
        private readonly ConsoleView _consoleView;
        private readonly GameSpeedView _gameSpeedView;

        public GameView(
            GameViewModel gameViewModel,
            InGameOptionsView inGameOptionsView,
            ConsoleView consoleView,
            GameSpeedView gameSpeedView
        )
            : base(gameViewModel)
        {
            _inGameOptionsView = inGameOptionsView;
            _consoleView = consoleView;
            _gameSpeedView = gameSpeedView;
            _components.Add(_gameSpeedView);
        }

        protected override void InitializeInternal()
        {
            SetupInGameOptions();
            SetupConsole();
            SetupGameSpeedView();
        }

        private void SetupGameSpeedView()
        {
            _gameSpeedView.Initialize();

            var timePanel = new Panel(
                new Vector2(300f, 110f),
                PanelSkin.Simple,
                Anchor.TopRight)
                .NoPadding();

            timePanel.Opacity = 50;

            _gameSpeedView.RootPanel.AddAsChildOf(timePanel);

            RootPanel.AddChild(timePanel);
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

            _inGameOptionsView.Initialize();

            RootPanel.AddChild(_inGameOptionsView.RootPanel);
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

        public bool IsMouseIn3DView => RootPanel != null && RootPanel.IsMouseInRootPanelEmptySpace;

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
    }
}