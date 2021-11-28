using MarsUndiscovered.Graphics.Camera;
using MarsUndiscovered.Interfaces;
using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Views;

using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.Monogame.Core.View;

namespace MarsUndiscovered.UserInterface.Screens
{
    public class GameScreen : Screen
    {
        private readonly GameView3D _gameView3D;
        private readonly IGameTimeService _gameTimeService;
        private IMarsUndiscoveredGameWorld _marsUndiscoveredGameWorld;

        public GameScreen(
            IMarsUndiscoveredGameWorld marsUndiscoveredGameWorld,
            GameView gameView,
            GameView3D gameView3D,
            IGameTimeService gameTimeService
            ) : base(gameView)
        {
            _marsUndiscoveredGameWorld = marsUndiscoveredGameWorld;
            _gameView3D = gameView3D;
            _gameTimeService = gameTimeService;
        }

        public void EndGame()
        {
            _gameTimeService.Stop();
        }

        public void StartNewGame()
        {
            _marsUndiscoveredGameWorld.StartNewGame();
            _gameView3D.StartNewGame();

            _gameTimeService.Start();
        }

        public override void Update()
        {
            _marsUndiscoveredGameWorld.Update();
            _gameView3D.Update();
        }

        public override void Draw()
        {
            _gameView3D.Draw();
            base.Draw();
        }
    }
}