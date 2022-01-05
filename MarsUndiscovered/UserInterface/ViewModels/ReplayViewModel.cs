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
            Notify();
        }

        private void SetupNewReplay()
        {
            MapViewModel.SetupNewMap(GameWorld);
            _messageLogCount = 0;
            TurnNumber = 1;
        }

        public void ExecuteNextReplayCommand()
        {
            GameWorld.ExecuteNextReplayCommand();
            TurnNumber++;
            Notify();
        }
    }
}