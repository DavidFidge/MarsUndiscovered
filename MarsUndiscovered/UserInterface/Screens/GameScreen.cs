using Augmented.Graphics.Camera;
using Augmented.Interfaces;
using Augmented.Messages;
using Augmented.UserInterface.Views;

using DavidFidge.MonoGame.Core.Interfaces.Services;
using DavidFidge.Monogame.Core.View;

namespace Augmented.UserInterface.Screens
{
    public class GameScreen : Screen
    {
        private readonly GameView3D _gameView3D;
        private readonly IGameTimeService _gameTimeService;
        private IAugmentedGameWorld _augmentedGameWorld;

        public GameScreen(
            IAugmentedGameWorld augmentedGameWorld,
            GameView gameView,
            GameView3D gameView3D,
            IGameTimeService gameTimeService
            ) : base(gameView)
        {
            _augmentedGameWorld = augmentedGameWorld;
            _gameView3D = gameView3D;
            _gameTimeService = gameTimeService;
        }

        public void EndGame()
        {
            _gameTimeService.Stop();
        }

        public void StartNewGame()
        {
            _augmentedGameWorld.StartNewGame();
            _gameView3D.StartNewGame();

            Mediator.Send(new ChangeGameSpeedRequest().ResetRequest());

            _gameTimeService.Start();
        }

        public override void Update()
        {
            _augmentedGameWorld.Update();
            _gameView3D.Update();
        }

        public override void Draw()
        {
            _gameView3D.Draw();
            base.Draw();
        }
    }
}