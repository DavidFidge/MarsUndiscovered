using System.Threading;
using System.Threading.Tasks;
using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.MonoGame.Core.Graphics.Camera;
using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

using FrigidRogue.MonoGame.Core.View.Extensions;

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
        IRequestHandler<RightClickViewRequest>
    {
        public bool IsMouseInGameView => RootPanel?.IsMouseInRootPanelEmptySpace ?? true;

        private readonly InGameOptionsView _inGameOptionsView;
        private readonly ConsoleView _consoleView;
        private readonly IGameCamera _gameCamera;
        private SpriteBatch _spriteBatch;

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
        }

        protected override void InitializeInternal()
        {
            SetupInGameOptions();
            SetupConsole();

            _gameCamera.Initialise();

            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
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
            var cellSize = new Point(20, 20);

            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            var wallString = "#";
            var floorString = "·";

            for (var x = 0; x < Data.WallsFloors.Width; x++)
            {
                for (var y = 0; y < Data.WallsFloors.Height; y++)
                {
                    var xd = Data.WallsFloors[x, y];

                    _spriteBatch.DrawString(Assets.MapFont, xd ? floorString : wallString, new Vector2(x * cellSize.X, y * cellSize.Y), Color.White);
                }
            }

            _spriteBatch.End();

            base.Draw();
        }

        public override void Update()
        {
            _gameCamera.Update();
            base.Update();
        }

        public Task<Unit> Handle(LeftClickViewRequest request, CancellationToken cancellationToken)
        {
            return Unit.Task;
        }

        public Task<Unit> Handle(RightClickViewRequest request, CancellationToken cancellationToken)
        {
            return Unit.Task;
        }
    }
}