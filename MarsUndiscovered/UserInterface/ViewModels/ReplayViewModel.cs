using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class ReplayViewModel : BaseGameViewModel<ReplayData>
    {
        public int TurnNumber { get; set; }

        public void LoadReplay(string filename)
        {
            IsActive = true;
            GameWorldProvider.LoadReplay(filename);
            SetupNewReplay();
            Mediator.Publish(new RefreshViewNotification());
        }

        private void SetupNewReplay()
        {
            SetUpViewModels();
            TurnNumber = 1;
        }

        public bool ExecuteNextReplayCommand()
        {
            var wasCommandExecuted = GameWorld.ExecuteNextReplayCommand();

            if (!wasCommandExecuted)
                return false;

            TurnNumber++;
            Mediator.Publish(new RefreshViewNotification());

            return true;
        }
    }
}