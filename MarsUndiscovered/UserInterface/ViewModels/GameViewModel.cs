using FrigidRogue.MonoGame.Core.Extensions;

using GoRogue.Pathing;

using MarsUndiscovered.Components;
using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;

using Microsoft.Xna.Framework;

using SadRogue.Primitives;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class GameViewModel : BaseGameViewModel<GameData>
    {
        public void NewGame(ulong? seed = null)
        {
            IsActive = true;
            GameWorldEndpoint.NewGame(seed);
            SetUpViewModels();
            GameWorldEndpoint.AfterCreateGame();
            Mediator.Publish(new RefreshViewNotification());
        }

        public void LoadGame(string filename)
        {
            IsActive = true;
            GameWorldEndpoint.LoadGame(filename);
            SetUpViewModels();
            GameWorldEndpoint.AfterCreateGame();
            Mediator.Publish(new RefreshViewNotification());
        }

        public void Move(Direction direction)
        {
            GameWorld.MoveRequest(direction);
            Mediator.Publish(new RefreshViewNotification());
        }

        public AutoExploreResult AutoExplore()
        {
            var result = GameWorld.AutoExploreRequest();
            Mediator.Publish(new RefreshViewNotification());
            return result;
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

            Mediator.Publish(new RefreshViewNotification());

            if (path.Length == 1)
                return true;

            return false;
        }
    }
}