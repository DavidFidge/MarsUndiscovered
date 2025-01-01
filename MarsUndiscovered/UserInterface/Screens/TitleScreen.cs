using System.Threading;
using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.View;
using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Views;

namespace MarsUndiscovered.UserInterface.Screens
{
    public class TitleScreen : Screen,
        IRequestHandler<QuitToTitleRequest>
    {
        public TitleScreen(TitleView titleView) : base(titleView)
        {
        }

        public void Handle(QuitToTitleRequest request)
        {
            UserInterface.ShowScreen(this);
        }
    }
}