using System.Threading;
using System.Threading.Tasks;
using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.MonoGame.Core.Graphics.Camera;
using FrigidRogue.MonoGame.Core.Graphics.Quads;
using FrigidRogue.MonoGame.Core.View.Extensions;

using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

using GeonBit.UI.Entities;
using MarsUndiscovered.Interfaces;
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
        private RenderTarget2D _renderTarget;
        private TexturedQuadTemplate _texturedQuadTemplate;

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
            _gameCamera.MoveSensitivity = 0.001f;
            _gameCamera.ZoomSensitivity = 0.001f;
        }

        protected override void InitializeInternal()
        {
            SetupInGameOptions();
            SetupConsole();

            _gameCamera.Initialise();

            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            _texturedQuadTemplate = new TexturedQuadTemplate(GameProvider);

            _renderTarget = new RenderTarget2D(Game.GraphicsDevice,
                1024,
                1024,
                false,
                Game.GraphicsDevice.PresentationParameters.BackBufferFormat,
                Game.GraphicsDevice.PresentationParameters.DepthStencilFormat, 0,
                RenderTargetUsage.PreserveContents);

            _texturedQuadTemplate.LoadContent(2, 2, _renderTarget);
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
            var cellSize = 2f / Data.WallsFloors.Height;

            //Game.GraphicsDevice.SetRenderTarget(_renderTarget);

            for (var x = 0; x < Data.WallsFloors.Width; x++)
            {
                for (var y = 0; y < Data.WallsFloors.Height; y++)
                {
                    var xd = Data.WallsFloors[x, y];

                    //Assets.WallQuad.Draw(Matrix.Identity, Matrix.Identity, Matrix.CreateTranslation(x * 0.1f, y * -0.1f, 0));

                    var scale = Matrix.CreateScale(cellSize);
                    var localTranslation = Matrix.CreateTranslation(x * cellSize, y * cellSize, 0);
                    var worldTranslation = Matrix.CreateTranslation(-1, -1, -1);
                    var transform = Matrix.Multiply(Matrix.Multiply(scale, localTranslation), worldTranslation);

                    Assets.WallQuad.Draw(_gameCamera.View, _gameCamera.Projection, transform);

                }
            }

            //Game.GraphicsDevice.SetRenderTarget(null);

            //_texturedQuadTemplate.Draw(_gameCamera.View, _gameCamera.Projection, Matrix.CreateTranslation(0, 0, -1));

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