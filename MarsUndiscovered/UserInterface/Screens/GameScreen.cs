using MarsUndiscovered.Interfaces;
using MarsUndiscovered.UserInterface.Views;

using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.View;

namespace MarsUndiscovered.UserInterface.Screens
{
    public class GameScreen : Screen
    {
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
            _gameTimeService.Start();
        }

        public override void Update()
        {
            _marsUndiscoveredGameWorld.Update();
        }

        public override void Draw()
        {
            base.Draw();
        }
    }
}