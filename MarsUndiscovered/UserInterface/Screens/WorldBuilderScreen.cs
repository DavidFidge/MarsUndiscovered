using System.Threading;
using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.View;
using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Views;

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

        public void Handle(WorldBuilderRequest request)
        {
            _worldBuilderView.LoadWorldBuilder();

            UserInterface.ShowScreen(this);

        }
    }
}