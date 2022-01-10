using MarsUndiscovered.UserInterface.Data;

using Microsoft.Xna.Framework;

using SadRogue.Primitives;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class GameViewModel : BaseGameViewModel<GameData>
    {
        public void NewGame(uint? seed = null)
        {
            GameWorldProvider.NewGame(seed);
            SetUpViewModels();
            GetNewTurnData();
        }

        public void LoadGame(string filename)
        {
            GameWorldProvider.LoadGame(filename);
            SetUpViewModels();
            GetNewTurnData();
        }

        public void Move(Direction direction)
        {
            GameWorld.MoveRequest(direction);
            GetNewTurnData();
        }

        public void Move(Ray pointerRay)
        {
            var point = MapViewModel.MousePointerRayToMapPosition(pointerRay);

            if (point == null)
                return;

            GameWorld.MoveRequest(point.Value);

            GetNewTurnData();
        }
    }
}