using System.Threading;
using System.Threading.Tasks;
using MarsUndiscovered.UserInterface.Views;

using FrigidRogue.MonoGame.Core.View;
using MarsUndiscovered.Messages;
using MediatR;

namespace MarsUndiscovered.UserInterface.Screens
{
    public class WorldBuilderScreen : Screen,
        IRequestHandler<WorldBuilderRequest>
    {
        private readonly WorldBuilderView _worldBuilderView;

        public WorldBuilderScreen(WorldBuilderView worldBuilderView) : base(worldBuilderView)
        {
            _worldBuilderView = worldBuilderView;
        }

        public Task<Unit> Handle(WorldBuilderRequest request, CancellationToken cancellationToken)
        {
            _worldBuilderView.LoadWorldBuilder();

            UserInterface.ShowScreen(this);

            return Unit.Task;
        }
    }
}