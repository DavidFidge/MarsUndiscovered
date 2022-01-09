using System.Threading;
using System.Threading.Tasks;
using MarsUndiscovered.UserInterface.Views;

using FrigidRogue.MonoGame.Core.View;
using MarsUndiscovered.Messages;
using MediatR;

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

        public Task<Unit> Handle(NewGameRequest request, CancellationToken cancellationToken)
        {
            UserInterface.ShowScreen(this);

            _gameView.NewGame(request.Seed);

            return Unit.Task;
        }

        public Task<Unit> Handle(LoadGameRequest request, CancellationToken cancellationToken)
        {
            UserInterface.ShowScreen(this);

            _gameView.LoadGame(request.Filename);

            return Unit.Task;
        }
    }
}