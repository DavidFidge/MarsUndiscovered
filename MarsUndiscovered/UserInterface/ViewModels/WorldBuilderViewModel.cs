using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class WorldBuilderViewModel : BaseGameViewModel<WorldBuilderData>
    {
        public bool BuildWorld()
        {
            IsActive = true;
            GameWorldEndpoint.NewWorldBuilder();
            SetUpViewModels();
            GameWorldEndpoint.AfterCreateWorldBuilder();

            Mediator.Publish(new RefreshViewNotification());

            return true;
        }
    }
}