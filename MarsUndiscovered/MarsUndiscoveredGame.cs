using System.Threading;
using System.Threading.Tasks;
using FrigidRogue.MonoGame.Core.Extensions;
using MarsUndiscovered.Interfaces;
using MarsUndiscovered.UserInterface.Data;

using FrigidRogue.MonoGame.Core.Graphics;
using FrigidRogue.MonoGame.Core.Graphics.Camera;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Messages;
using FrigidRogue.MonoGame.Core.Services;
using FrigidRogue.MonoGame.Core.View.Interfaces;

using MarsUndiscovered.Components;
using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Input;
using MediatR;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Serilog;

namespace MarsUndiscovered
{
    public class MarsUndiscoveredGame : Game, IGame
    {
        private readonly ILogger _logger;
        private readonly IGameProvider _gameProvider;
        private readonly IGameTimeService _gameTimeService;
        private readonly IGameInputService _gameInputService;
        private readonly IUserInterface _userInterface;
        private readonly IGameOptionsStore _gameOptionsStore;
        private readonly IAssets _assets;
        private readonly ScreenCollection _screenCollection;
        private readonly IGameCamera _gameCamera;
        private readonly IMediator _mediator;
        private readonly Options _options;

        private bool _isExiting;
        private bool _startNewGameFromCommandLine = false;
        private bool _startWorldBuilderFromCommandLine = false;

        public CustomGraphicsDeviceManager CustomGraphicsDeviceManager { get; }
        public EffectCollection EffectCollection
        { get; }
        private SpriteBatch _spriteBatch;
        private RenderTarget2D _renderTarget;
        
        private FpsCounter _fpsCounter = new();

        public MarsUndiscoveredGame(
            ILogger logger,
            IGameProvider gameProvider,
            IGameTimeService gameTimeService,
            IGameInputService gameInputService,
            IUserInterface userInterface,
            IGameOptionsStore gameOptionsStore,
            IAssets assets,
            ScreenCollection screenCollection,
            IGameCamera gameCamera,
            Options options,
            IMediator mediator,
            GlobalKeyboardHandler globalKeyboardHandler
        )
        {
            _logger = logger;
            _gameProvider = gameProvider;
            _gameProvider.Game = this;
            _gameTimeService = gameTimeService;
            _gameInputService = gameInputService;
            _userInterface = userInterface;
            _gameOptionsStore = gameOptionsStore;
            _logger.Debug("Starting game");

            CustomGraphicsDeviceManager = new CustomGraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            _assets = assets;
            _screenCollection = screenCollection;
            _gameCamera = gameCamera;
            _mediator = mediator;
            _options = options;

            if (_options.NewGame)
                _startNewGameFromCommandLine = true;
            else if (_options.WorldBuilder)
                _startWorldBuilderFromCommandLine = true;

            EffectCollection = new EffectCollection(_gameProvider);

            Window.AllowUserResizing = true;
            _gameInputService.AddGlobalKeyboardHandler(globalKeyboardHandler);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            _gameCamera.Initialise();
            EffectCollection.Initialize();

            _userInterface.Initialize(Content, "mars");

            var gameOptions = _gameOptionsStore.GetFromStore<VideoOptionsData>()?.State;

            _userInterface.RenderResolution = gameOptions?.SelectedRenderResolution;
            _gameCamera.RenderResolution = gameOptions?.SelectedRenderResolution;
            _gameCamera.MoveSensitivity = 1f;
            _gameCamera.RotateSensitivity = 0.01f;
            _gameCamera.ZoomSensitivity = 0.01f;

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            InitializeDisplaySettings();

            _screenCollection.Initialize();

            _userInterface.ShowScreen(_screenCollection.StartupScreen);

            _renderTarget = new RenderTarget2D(
                _gameProvider.Game.GraphicsDevice,
                gameOptions?.SelectedRenderResolution.Width ?? 3840,
                gameOptions?.SelectedRenderResolution.Height ?? 2160,
                false,
                _gameProvider.Game.GraphicsDevice.PresentationParameters.BackBufferFormat,
                _gameProvider.Game.GraphicsDevice.PresentationParameters.DepthStencilFormat,
                0,
                RenderTargetUsage.PreserveContents
            );

            base.Initialize();
        }

        private void InitializeDisplaySettings()
        {
            var isFullScreen = false;
            var isVerticalSync = true;
            var isBorderlessWindowed = true;

            var gameOptions = _gameOptionsStore.GetFromStore<VideoOptionsData>()?.State;

            var displayDimensions = new DisplayDimension(
                CustomGraphicsDeviceManager.GraphicsDevice.Adapter.CurrentDisplayMode.Width,
                CustomGraphicsDeviceManager.GraphicsDevice.Adapter.CurrentDisplayMode.Height,
                0);

            if (gameOptions?.SelectedDisplayDimension != null)
            {
                var displayModes = CustomGraphicsDeviceManager.GetSupportedDisplayModes();

                if (displayModes.Any(dm => Equals(dm, gameOptions.SelectedDisplayDimension)))
                {
                    displayDimensions = gameOptions.SelectedDisplayDimension;
                    isFullScreen = gameOptions.IsFullScreen;
                    isVerticalSync = gameOptions.IsVerticalSync;
                    isBorderlessWindowed = gameOptions.IsBorderlessWindowed;
                }
            }

            var displaySettings = new DisplaySettings(
                displayDimensions,
                isFullScreen,
                isVerticalSync,
                isBorderlessWindowed
            );

            CustomGraphicsDeviceManager.SetDisplayMode(displaySettings);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            _assets.LoadContent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (_isExiting)
                Exit();

            if (_startNewGameFromCommandLine && _options.NewGame)
            {
                _mediator.Send(new NewGameRequest());
                _startNewGameFromCommandLine = false;
            }
            else if (_startWorldBuilderFromCommandLine && _options.WorldBuilder)
            {
                _mediator.Send(new WorldBuilderRequest());
                _startWorldBuilderFromCommandLine = false;
            }

            _gameTimeService.Update(gameTime);
            _gameInputService.Poll(GraphicsDevice.Viewport.Bounds);
            _userInterface.Update(gameTime);
            _fpsCounter.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            _gameProvider.Game.GraphicsDevice.RestoreGraphicsDeviceAfterSpriteBatchDraw();

            GeonBit.UI.UserInterface.Active.Draw(_spriteBatch);

            _gameProvider.Game.GraphicsDevice.SetRenderTarget(_renderTarget);

            _gameProvider.Game.GraphicsDevice.RestoreGraphicsDeviceAfterSpriteBatchDraw();

            _gameProvider.Game.GraphicsDevice.Clear(Color.Transparent);

            _userInterface.DrawActiveScreen();

            _gameProvider.Game.GraphicsDevice.SetRenderTarget(null);

            _gameProvider.Game.GraphicsDevice.RestoreGraphicsDeviceAfterSpriteBatchDraw();

            DrawRenderTarget(_spriteBatch);

            GeonBit.UI.UserInterface.Active.DrawMainRenderTarget(_spriteBatch);
            
            _spriteBatch.Begin();

            // Draw the fps msg
            _fpsCounter.DrawFps(_spriteBatch, _assets.MapFont, new Vector2(1f, 1f), Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public void DrawRenderTarget(SpriteBatch spriteBatch)
        {
            var viewportWidth = spriteBatch.GraphicsDevice.Viewport.Width;
            var viewportHeight = spriteBatch.GraphicsDevice.Viewport.Height;

            // draw the main render target
            if (_renderTarget != null && !_renderTarget.IsDisposed)
            {
                // draw render target
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                spriteBatch.Draw(_renderTarget, new Rectangle(0, 0, viewportWidth, viewportHeight), Color.White);
                spriteBatch.End();
            }
        }

        public Task<Unit> Handle(QuitToDesktopRequest request, CancellationToken cancellationToken)
        {
            _isExiting = true;
            return Unit.Task;
        }
        
        public Task<Unit> Handle(ToggleFullScreenRequest request, CancellationToken cancellationToken)
        {
            CustomGraphicsDeviceManager.ToggleFullScreen();
            return Unit.Task;
        }
    }
}
