using System;
using System.Threading;
using System.Threading.Tasks;

using Augmented.Messages;
using Augmented.UserInterface.Data;

using DavidFidge.MonoGame.Core.Interfaces.Services;
using DavidFidge.MonoGame.Core.Messages;
using DavidFidge.MonoGame.Core.UserInterface;

using MediatR;

namespace Augmented.UserInterface.ViewModels
{
    public class GameSpeedViewModel : BaseViewModel<GameSpeedData>,
        INotificationHandler<GameTimeUpdateNotification>,
        IRequestHandler<ChangeGameSpeedRequest>
    {
        private readonly IGameTimeService _gameTimeService;

        public GameSpeedViewModel(IGameTimeService gameTimeService)
        {
            _gameTimeService = gameTimeService;
        }

        public Task Handle(GameTimeUpdateNotification notification, CancellationToken cancellationToken)
        {
            if (Data.TotalGameTime != notification.GameTime.TotalGameTime)
            {
                Data.TotalGameTime = notification.GameTime.TotalGameTime;

                Mediator.Send(new UpdateViewRequest<GameSpeedData>());
            }

            return Unit.Task;
        }

        public Task<Unit> Handle(ChangeGameSpeedRequest request, CancellationToken cancellationToken)
        {
            if (request.TogglePauseGame)
            {
                if (_gameTimeService.IsPaused)
                    _gameTimeService.ResumeGame();
                else
                    _gameTimeService.PauseGame();
            }
            else if (request.Reset)
            {
                _gameTimeService.Reset();
            }
            else
            {
                var incrementAbs = Math.Abs(request.Increment);

                while (incrementAbs > 0)
                {
                    if (request.Increment > 0)
                        _gameTimeService.IncreaseGameSpeed();
                    else
                        _gameTimeService.DecreaseGameSpeed();

                    incrementAbs--;
                }
            }

            Data.IsPaused = _gameTimeService.IsPaused;
            Data.GameSpeedPercent = _gameTimeService.GameSpeedPercent;

            Mediator.Send(new UpdateViewRequest<GameSpeedData>());

            return Unit.Task;
        }
    }
}