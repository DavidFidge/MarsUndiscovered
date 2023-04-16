using System.Threading;
using System.Threading.Tasks;
using FrigidRogue.MonoGame.Core.View;
using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Views;
using MediatR;

namespace MarsUndiscovered.UserInterface.Screens
{
    public class WaveFunctionCollapseScreen : Screen,
        IRequestHandler<WaveFunctionCollapseRequest>
    {
        private readonly WaveFunctionCollapseView _waveFunctionCollapseView;

        public WaveFunctionCollapseScreen(WaveFunctionCollapseView waveFunctionCollapseView) : base(waveFunctionCollapseView)
        {
            _waveFunctionCollapseView = waveFunctionCollapseView;
        }

        public Task<Unit> Handle(WaveFunctionCollapseRequest request, CancellationToken cancellationToken)
        {
            _waveFunctionCollapseView.LoadWaveFunctionCollapse();

            UserInterface.ShowScreen(this);

            return Unit.Task;
        }
    }
}