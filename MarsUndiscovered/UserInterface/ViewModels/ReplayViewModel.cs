using MarsUndiscovered.UserInterface.Data;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class ReplayViewModel : BaseGameViewModel<ReplayData>
    {
        public void LoadReplay(string filename)
        {
            GameWorldProvider.LoadReplay(filename);
            SetupNewReplay();
        }

        private void SetupNewReplay()
        {
            MapViewModel.SetupNewMap(GameWorld);

            _messageLogCount = 0;
        }

        public void ExecuteNextReplayCommand()
        {
            GameWorld.ExecuteNextReplayCommand();
        }
    }
}