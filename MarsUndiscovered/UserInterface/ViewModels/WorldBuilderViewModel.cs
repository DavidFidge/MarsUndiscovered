using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class WorldBuilderViewModel : BaseGameCoreViewModel<WorldBuilderData>
    {
        public bool BuildWorld()
        {
            IsActive = true;
            GameWorldEndpoint.NewWorldBuilder();
            SetUpGameCoreViewModels();
            GameWorldEndpoint.AfterCreateWorldBuilder();

            Mediator.Publish(new RefreshViewNotification());

            return true;
        }

        protected override void RefreshView()
        {
        }
    }
}