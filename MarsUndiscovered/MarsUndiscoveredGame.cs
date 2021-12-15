using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MarsUndiscovered.Interfaces;
using MarsUndiscovered.UserInterface.Data;

using FrigidRogue.MonoGame.Core.Graphics;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Messages;
using FrigidRogue.MonoGame.Core.Services;
using FrigidRogue.MonoGame.Core.View;
using FrigidRogue.MonoGame.Core.View.Interfaces;
using MarsUndiscovered.Components;
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

        private bool _isExiting;

        public CustomGraphicsDeviceManager CustomGraphicsDeviceManager { get; }
        public EffectCollection EffectCollection
        { get; }
        private SpriteBatch _spriteBatch;

        public MarsUndiscoveredGame(
            ILogger logger,
            IGameProvider gameProvider,
            IGameTimeService gameTimeService,
            IGameInputService gameInputService,
            IUserInterface userInterface,
            IGameOptionsStore gameOptionsStore,
            IAssets assets,
            ScreenCollection screenCollection
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

            EffectCollection = new EffectCollection(_gameProvider);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            EffectCollection.Initialize();

            _userInterface.Initialize(Content, "mars");

            var gameOptions = _gameOptionsStore.GetFromStore<VideoOptionsData>()?.State;

            _userInterface.RenderResolution = gameOptions?.SelectedRenderResolution;

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            InitializeDisplaySettings();

            _screenCollection.Initialize();

            _userInterface.ShowScreen(_screenCollection.StartupScreen);

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

            _gameTimeService.Update(gameTime);
            _gameInputService.Poll();
            _userInterface.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            _userInterface.Draw(_spriteBatch);

            base.Draw(gameTime);
        }

        public Task<Unit> Handle(QuitToDesktopRequest request, CancellationToken cancellationToken)
        {
            _isExiting = true;
            return Unit.Task;
        }
    }
}
