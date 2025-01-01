using System.Threading;
using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.View;
using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Views;

namespace MarsUndiscovered.UserInterface.Screens
{
    public class GameScreen : Screen,
        IRequestHandler<NewGameRequest>,
        IRequestHandler<LoadGameRequest>
    {
        private readonly GameView _gameView;

        public GameScreen(GameView gameView) : base(gameView)
        {
            _gameView = gameView;
        }

        public void Handle(NewGameRequest request)
        {
            UserInterface.ShowScreen(this);

            _gameView.NewGame(request.Seed);
        }

        public void Handle(LoadGameRequest request)
        {
            UserInterface.ShowScreen(this);

            _gameView.LoadGame(request.Filename);
        }
    }
}