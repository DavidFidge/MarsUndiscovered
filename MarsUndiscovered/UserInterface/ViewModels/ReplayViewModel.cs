using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class ReplayViewModel : BaseGameViewModel<ReplayData>
    {
        public int TurnNumber { get; set; }

        public void LoadReplay(string filename)
        {
            IsActive = false;
            GameWorldEndpoint.LoadReplay(filename);
            IsActive = true;
            SetupNewReplay();
            MapViewModel.RecentreMap();
            Mediator.Publish(new RefreshViewNotification());
        }

        private void SetupNewReplay()
        {
            SetUpViewModels();
            GameWorldEndpoint.AfterCreateGame();
            TurnNumber = 1;
        }

        public bool ExecuteNextReplayCommand()
        {
            var wasCommandExecuted = GameWorldEndpoint.ExecuteNextReplayCommand();

            if (!wasCommandExecuted)
                return false;

            TurnNumber++;
            MapViewModel.RecentreMap();
            Mediator.Publish(new RefreshViewNotification());

            return true;
        }
    }
}