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
        IRequestHandler<EndCurrentGameRequest>
    {
        private readonly GameView _gameView;

        public GameScreen(GameView gameView) : base(gameView)
        {
            _gameView = gameView;
        }

        public Task<Unit> Handle(NewGameRequest request, CancellationToken cancellationToken)
        {
            _gameView.CreateGame();

            UserInterface.ShowScreen(this);

            return Unit.Task;
        }

        public Task<Unit> Handle(EndCurrentGameRequest request, CancellationToken cancellationToken)
        {
            Mediator.Send(new QuitToTitleRequest());

            return Unit.Task;
        }
    }
}