using System.Threading;
using System.Threading.Tasks;

using FrigidRogue.MonoGame.Core.View;

using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Views;

using MediatR;

namespace MarsUndiscovered.UserInterface.Screens
{
    public class TitleScreen : Screen,
        IRequestHandler<QuitToTitleRequest>
    {
        public TitleScreen(TitleView titleView) : base(titleView)
        {
        }

        public Task<Unit> Handle(QuitToTitleRequest request, CancellationToken cancellationToken)
        {
            UserInterface.ShowScreen(this);

            return Unit.Task;
        }
    }
}