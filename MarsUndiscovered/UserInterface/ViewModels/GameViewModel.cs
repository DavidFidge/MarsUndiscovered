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
            IsActive = false;
            GameWorldEndpoint.NewGame(seed);
            IsActive = true;
            SetUpViewModels();
            GameWorldEndpoint.AfterCreateGame();
            Mediator.Publish(new RefreshViewNotification());
        }

        public void LoadGame(string filename)
        {
            IsActive = false;
            GameWorldEndpoint.LoadGame(filename);
            IsActive = true;
            SetUpViewModels();
            GameWorldEndpoint.AfterCreateGame();
            Mediator.Publish(new RefreshViewNotification());
        }

        public void Move(Direction direction)
        {
            if (Animations.Any())
                return;

            var commandResult = GameWorldEndpoint.MoveRequest(direction);
            QueueAnimations(commandResult);
            Mediator.Publish(new RefreshViewNotification());
        }

        public AutoExploreResult AutoExplore()
        {
            var result = GameWorldEndpoint.AutoExploreRequest();
            QueueAnimations(result.CommandResults);
            Mediator.Publish(new RefreshViewNotification());
            return result;
        }

        public Path GetPathToDestination(Ray pointerRay)
        {
            var point = MapViewModel.MousePointerRayToMapPosition(pointerRay);

            if (point == null)
                return null;

            return GameWorldEndpoint.GetPathToPlayer(point.Value);
        }

        public bool Move(Path path)
        {
            if (GameWorldEndpoint.GetPlayerPosition().Equals(path.End))
                return true;

            if (Animations.Any())
                return false;

            var result = GameWorldEndpoint.MoveRequest(path);

            QueueAnimations(result);

            if (result.IsEmpty())
                return true;

            Mediator.Publish(new RefreshViewNotification());

            if (path.Length == 1)
                return true;

            return false;
        }
    }
}
