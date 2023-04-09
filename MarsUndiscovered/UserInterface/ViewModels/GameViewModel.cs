using System.Threading.Tasks;
using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using GoRogue.Pathing;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;

using Microsoft.Xna.Framework;

using SadRogue.Primitives;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class GameViewModel : BaseGameViewModel<GameData>
    {
        private readonly IGameOptionsStore _gameOptionsStore;

        public GameViewModel(IGameOptionsStore gameOptionsStore)
        {
            _gameOptionsStore = gameOptionsStore;
        }
        
        public void NewGame(ulong? seed = null)
        {
            IsActive = false;
            GameWorldEndpoint.NewGame(seed);
            IsActive = true;
            SetUpViewModels();
            GameWorldEndpoint.AfterCreateGame();
            MapViewModel.RecentreMap();
            Mediator.Publish(new RefreshViewNotification());
        }

        public void LoadGame(string filename)
        {
            IsActive = false;
            GameWorldEndpoint.LoadGame(filename);
            IsActive = true;
            SetUpViewModels();
            GameWorldEndpoint.AfterCreateGame();
            MapViewModel.RecentreMap();
            Mediator.Publish(new RefreshViewNotification());
        }

        public void Move(Direction direction)
        {
            if (Animations.Any())
                return;

            var commandResult = GameWorldEndpoint.MoveRequest(direction);
            QueueAnimations(commandResult);
            MapViewModel.RecentreMap();
            Mediator.Publish(new RefreshViewNotification());
        }

        public AutoExploreResult AutoExplore()
        {
            var result = GameWorldEndpoint.AutoExploreRequest();
            QueueAnimations(result.CommandResults);
            MapViewModel.RecentreMap();
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
            var currentPlayerPosition = GameWorldEndpoint.GetPlayerPosition();
            
            if (currentPlayerPosition.Equals(path.End))
                return true;

            var result = GameWorldEndpoint.MoveRequest(path);

            QueueAnimations(result);
            
            MapViewModel.RecentreMap();
            Mediator.Publish(new RefreshViewNotification());
            
            if (result.IsEmpty())
                return true;

            if (path.Length == 1)
                return true;

            var newPlayerPosition = GameWorldEndpoint.GetPlayerPosition();

            if (currentPlayerPosition.Equals(newPlayerPosition))
                return true;

            return false;
        }

        public void WriteAndSendMorgue()
        {
            var gameId = GameWorldEndpoint.GetGameId();
            var gameOptionsStore = _gameOptionsStore.GetFromStore<GameOptionsData>();

            GameWorldEndpoint.SnapshotMorgue(gameOptionsStore.State.MorgueUsername ?? String.Empty);

            Task.Run(() => GameWorldEndpoint.WriteMorgueToFile(gameId));

            if (gameOptionsStore.State.UploadMorgueFiles)
                Task.Run(() => GameWorldEndpoint.SendMorgueToWeb(gameId));
        }
    }
}
