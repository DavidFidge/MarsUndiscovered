using System.Threading.Tasks;
using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Extensions;
using GoRogue.Pathing;
using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
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
            FinishAnimations();
            var commandResult = GameWorldEndpoint.MoveRequest(direction);
            AfterTurnExecuted(commandResult);
        }

        public AutoExploreResult AutoExplore()
        {
            var result = GameWorldEndpoint.AutoExploreRequest();
            AfterTurnExecuted(result.CommandResults);
            return result;
        }

        private void AfterTurnExecuted(IList<CommandResult> commandResults)
        {
            QueueAnimations(commandResults);
            MapViewModel.RecentreMap();
            Mediator.Publish(new RefreshViewNotification());

            foreach (var commandResult in commandResults)
            {
                if (commandResult.Command.RequiresPlayerInput)
                {
                    if (commandResult.Command is ApplyMachineCommand applyMachineCommand)
                    {
                        if (applyMachineCommand.Machine.MachineType == MachineType.Analyzer)
                        {
                            Mediator.Send(new OpenGameInventoryRequest(InventoryMode.Identify));
                        }
                    }
                }
            }
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

            AfterTurnExecuted(result);

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
            var gameOptionsStore = GameOptionsStore.GetFromStore<GameOptionsData>();

            GameWorldEndpoint.SnapshotMorgue(gameOptionsStore.State.MorgueUsername ?? String.Empty, gameOptionsStore.State.UploadMorgueFiles);

            if (gameOptionsStore.State.UploadMorgueFiles)
                Task.Run(() => GameWorldEndpoint.SendPendingMorgues());
        }

        public void ForceNextLevel()
        {
            var result = GameWorldEndpoint.ForceLevelChange(ForceLevelChange.NextLevel);
            AfterTurnExecuted(result);
        }

        public void ForcePreviousLevel()
        {
            var result = GameWorldEndpoint.ForceLevelChange(ForceLevelChange.PreviousLevel);
            AfterTurnExecuted(result);
        }

        public void ApplyRequest(Keys requestKey)
        {
            this.GameWorldEndpoint.ApplyItemRequest(requestKey);
        }

        public List<InventoryItem> GetHotBarItems()
        {
            return GameWorldEndpoint.GetHotBarItems();
        }
    }
}
