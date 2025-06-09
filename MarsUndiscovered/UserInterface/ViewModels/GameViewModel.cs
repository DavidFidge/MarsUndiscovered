using System.Threading.Tasks;
using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Extensions;
using GoRogue.Pathing;
using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components;
using MarsUndiscovered.Game.Components.Dto;
using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SadRogue.Primitives;
using Point = SadRogue.Primitives.Point;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class GameViewModel : BaseGameCoreViewModel<GameData>
    {
        public uint? CurrentSquareChoiceMonsterId { get; set; }
        public uint? RetainedSquareChoiceMonsterId { get; set; }
        
        private List<RadioCommsItem> _radioCommsItems = new();
        public PlayerStatus PlayerStatus { get; set; }
        public IList<MonsterStatus> MonsterStatusInView { get; set; }

        public MessagesStatus MessageStatus { get; set; }
        
        protected void SetUpViewModels()
        {
            MessageStatus = new MessagesStatus();
            SetUpGameCoreViewModels();
        }

        protected override void RefreshView()
        {
            MonsterStatusInView = GameWorldProvider.GameWorld.GetStatusOfMonstersInView();
            PlayerStatus = GameWorldProvider.GameWorld.GetPlayerStatus();
            _radioCommsItems.AddRange(GameWorldProvider.GameWorld.GetNewRadioCommsItems());
            MessageStatus.AddMessages(GameWorldProvider.GameWorld.GetMessagesSince(MessageStatus.SeenMessageCount));
            MapViewModel.UpdateDebugTiles();
        }

        public IList<RadioCommsItem> GetNewRadioCommsItems()
        {
            var radioCommsItems = _radioCommsItems.ToList();
            _radioCommsItems.Clear();
            return radioCommsItems;
        }

        protected void FinishAnimations()
        {
            while (Animations.Any())
            {
                var animation = Animations.Dequeue();
                animation.Finish(MapViewModel);
            }
        }
        
        public void NewGame(ulong? seed = null)
        {
            IsActive = false;
            GameWorldProvider.NewGame(seed);
            IsActive = true;
            SetUpViewModels();
            GameWorldProvider.GameWorld.AfterCreateGame();
            MapViewModel.RecentreMap();
            Mediator.Publish(new RefreshViewNotification());
        }

        public void LoadGame(string filename)
        {
            IsActive = false;
            GameWorldProvider.LoadGame(filename);
            IsActive = true;
            SetUpViewModels();
            GameWorldProvider.GameWorld.AfterCreateGame();
            MapViewModel.RecentreMap();
            Mediator.Publish(new RefreshViewNotification());
        }

        public void Move(Direction direction)
        {
            FinishAnimations();
            var commandResult = GameWorldProvider.GameWorld.MoveRequest(direction);
            AfterTurnExecuted(commandResult);
        }

        public AutoExploreResult AutoExplore()
        {
            var result = GameWorldProvider.GameWorld.AutoExploreRequest();
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

            return GameWorldProvider.GameWorld.GetPathToPlayer(point.Value);
        }

        public bool Move(Path path)
        {
            var currentPlayerPosition = GameWorldProvider.GameWorld.GetPlayerPosition();
            
            if (currentPlayerPosition.Equals(path.End))
                return true;

            var result = GameWorldProvider.GameWorld.MoveRequest(path);

            AfterTurnExecuted(result);

            if (result.IsEmpty())
                return true;

            if (path.Length == 1)
                return true;

            var newPlayerPosition = GameWorldProvider.GameWorld.GetPlayerPosition();

            if (currentPlayerPosition.Equals(newPlayerPosition))
                return true;

            return false;
        }

        public void WriteAndSendMorgue()
        {
            var gameOptionsStore = GameOptionsStore.GetFromStore<GameOptionsData>();

            GameWorldProvider.GameWorld.SnapshotMorgue(gameOptionsStore.State.MorgueUsername ?? String.Empty, gameOptionsStore.State.UploadMorgueFiles);

            if (gameOptionsStore.State.UploadMorgueFiles)
                Task.Run(() => GameWorldProvider.GameWorld.SendPendingMorgues());
        }

        public void ForceNextLevel()
        {
            var result = GameWorldProvider.GameWorld.ForceLevelChange(ForceLevelChange.NextLevel);
            AfterTurnExecuted(result);
        }

        public void ForcePreviousLevel()
        {
            var result = GameWorldProvider.GameWorld.ForceLevelChange(ForceLevelChange.PreviousLevel);
            AfterTurnExecuted(result);
        }

        public void ApplyRequest(Keys requestKey)
        {
            var applyItemResults = GameWorldProvider.GameWorld.ApplyItemRequest(requestKey);
            AfterTurnExecuted(applyItemResults);
        }

        public List<InventoryItem> GetHotBarItems()
        {
            return GameWorldProvider.GameWorld.GetHotBarItems();
        }

        public void DoRangedAttack(InventoryItem selectedItem, Point target)
        {
            var result = GameWorldProvider.GameWorld.DoRangedAttack(selectedItem.Key, target);
            AfterTurnExecuted(result);
        }

        public InventoryItem GetEquippedWeapon()
        {
            return GameWorldProvider.GameWorld.GetEquippedItem();
        }

        public void MoveSquareChoice(Direction requestDirection)
        {
            MapViewModel.MoveHover(requestDirection);
        }
    }
}
