using MarsUndiscovered.UserInterface.Data;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class ReplayViewModel : BaseGameViewModel<ReplayData>
    {
        public int TurnNumber { get; set; }

        public void LoadReplay(string filename)
        {
            GameWorldProvider.LoadReplay(filename);
            SetupNewReplay();
            GetNewTurnData();
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
            GetNewTurnData();

            return true;
        }
    }
}