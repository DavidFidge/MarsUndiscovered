using GoRogue.Pathing;

using MarsUndiscovered.UserInterface.Data;

using Microsoft.Xna.Framework;

using NGenerics.Extensions;

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

        public Path GetPathToDestination(Ray pointerRay)
        {
            var point = MapViewModel.MousePointerRayToMapPosition(pointerRay);

            if (point == null)
                return null;

            return GameWorld.GetPathToPlayer(point.Value);
        }

        public bool Move(Path path)
        {
            if (GameWorld.Player.Position == path.End)
                return true;

            var result = GameWorld.MoveRequest(path);

            if (result.IsEmpty())
                return true;

            GetNewTurnData();

            if (path.Length == 1)
                return true;

            return false;
        }
    }
}