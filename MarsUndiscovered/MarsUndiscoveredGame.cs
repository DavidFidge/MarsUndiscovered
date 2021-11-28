using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MarsUndiscovered.Graphics;
using MarsUndiscovered.Graphics.Models;
using MarsUndiscovered.Interfaces;
using MarsUndiscovered.UserInterface.Data;

using FrigidRogue.MonoGame.Core.Graphics;
using FrigidRogue.MonoGame.Core.Graphics.Models;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Graphics;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Messages;
using FrigidRogue.MonoGame.Core.Services;
using FrigidRogue.Monogame.Core.View.Interfaces;

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
        private readonly IScreenManager _screenManager;
        private readonly IAssetProvider _assetProvider;

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
            IScreenManager screenManager,
            IAssetProvider assetProvider
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

            _screenManager = screenManager;
            _assetProvider = assetProvider;

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

            _userInterface.Initialize(Content);

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            InitializeDisplaySettings();

            _screenManager.Initialize();

            base.Initialize();
        }

        private void InitializeDisplaySettings()
        {
            var screenWidth = CustomGraphicsDeviceManager.GraphicsDevice.Adapter.CurrentDisplayMode.Width;
            var screenHeight = CustomGraphicsDeviceManager.GraphicsDevice.Adapter.CurrentDisplayMode.Height;
            var isFullScreen = false;
            var isVerticalSync = true;

            var gameOptions = _gameOptionsStore.GetFromStore<VideoOptionsData>()?.State;

            if (gameOptions != null && gameOptions.SelectedDisplayMode != null)
            {
                var displayModes = CustomGraphicsDeviceManager.GetSupportedDisplayModes();

                if (displayModes.Any(dm => Equals(dm, gameOptions.SelectedDisplayMode)))
                {
                    screenWidth = gameOptions.SelectedDisplayMode.Width;
                    screenHeight = gameOptions.SelectedDisplayMode.Height;
                    isFullScreen = gameOptions.IsFullScreen;
                    isVerticalSync = gameOptions.IsVerticalSync;
                }
            }

            CustomGraphicsDeviceManager.PreferredBackBufferWidth = screenWidth;
            CustomGraphicsDeviceManager.PreferredBackBufferHeight = screenHeight;
            CustomGraphicsDeviceManager.IsFullScreen = isFullScreen;
            CustomGraphicsDeviceManager.IsVerticalSync = isVerticalSync;
            CustomGraphicsDeviceManager.ApplyChanges();

            //GraphicsDevice.RasterizerState = new RasterizerState {
            //    CullMode = CullMode.None
            //};
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            _assetProvider.LoadContent();
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

            _userInterface.Update(gameTime);
            _gameTimeService.Update(gameTime);
            _gameInputService.Poll();
            _screenManager.Update();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            _userInterface.Draw(_spriteBatch);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Reset graphics device properties after SpriteBatch drawing
            // https://blogs.msdn.microsoft.com/shawnhar/2010/06/18/spritebatch-and-renderstates-in-xna-game-studio-4-0/
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            _screenManager.Draw();

            _userInterface.DrawMainRenderTarget(_spriteBatch);

            base.Draw(gameTime);
        }

        public Task<Unit> Handle(ExitGameRequest request, CancellationToken cancellationToken)
        {
            _isExiting = true;
            return Unit.Task;
        }
    }
}
