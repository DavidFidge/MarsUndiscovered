using MarsUndiscovered.UserInterface.Data;
using SadRogue.Primitives;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class GameViewModel : BaseGameViewModel<GameData>
    {
        public void NewGame(uint? seed = null)
        {
            GameWorldProvider.NewGame(seed);
            SetupNewGame();
            GetNewTurnData();
        }

        public void LoadGame(string filename)
        {
            GameWorldProvider.LoadGame(filename);
            SetupNewGame();
            GetNewTurnData();
        }

        public void Move(Direction direction)
        {
            GameWorld.MoveRequest(direction);
            GetNewTurnData();
        }
    }
}