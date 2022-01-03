using System.Threading;
using System.Threading.Tasks;
using MarsUndiscovered.UserInterface.Views;

using FrigidRogue.MonoGame.Core.View;
using MarsUndiscovered.Messages;
using MediatR;

namespace MarsUndiscovered.UserInterface.Screens
{
    public class ReplayScreen : Screen,
        IRequestHandler<LoadReplayRequest>
    {
        private readonly ReplayView _replayView;

        public ReplayScreen(ReplayView replayView) : base(replayView)
        {
            _replayView = replayView;
        }

        public Task<Unit> Handle(LoadReplayRequest request, CancellationToken cancellationToken)
        {
            _replayView.LoadReplay(request.Filename);

            UserInterface.ShowScreen(this);

            return Unit.Task;
        }
    }
}