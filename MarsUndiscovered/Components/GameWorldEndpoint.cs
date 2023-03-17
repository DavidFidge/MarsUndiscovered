using System.Threading.Tasks;
using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Services;

using GoRogue.GameFramework;
using GoRogue.Pathing;
using MarsUndiscovered.Components.Dto;
using MarsUndiscovered.Interfaces;

using Microsoft.Xna.Framework.Input;

using SadRogue.Primitives;

using Point = SadRogue.Primitives.Point;
using Rectangle = SadRogue.Primitives.Rectangle;

namespace MarsUndiscovered.Components
{
    public class GameWorldEndpoint : IGameWorldEndpoint, IGameWorldConsoleCommandEndpoint
    {
        private readonly IFactory<IGameWorld> _gameWorldFactory;

        public IGameWorld GameWorld { get; private set; }

        public GameWorldEndpoint(IFactory<IGameWorld> gameWorldFactory)
        {
            _gameWorldFactory = gameWorldFactory;
        }

        public SaveGameResult SaveGame(string saveGameName, bool overwrite)
        {
            return GameWorld.SaveGame(saveGameName, overwrite);
        }

        public void LoadGame(string gameName)
        {
            GameWorld = _gameWorldFactory.Create();
            GameWorld.LoadGame(gameName);
        }

        public void NewGame(ulong? seed = null)
        {
            GameWorld = _gameWorldFactory.Create();
            GameWorld.NewGame(seed);
        }

        public ProgressiveWorldGenerationResult ProgressiveWorldGeneration(ulong? seed, int step, WorldGenerationTypeParams worldGenerationTypeParams)
        {
            GameWorld = _gameWorldFactory.Create();
            return GameWorld.ProgressiveWorldGeneration(seed, step, worldGenerationTypeParams);
        }

        public void AfterCreateGame()
        {
            GameWorld.AfterCreateGame();
        }

        public List<InventoryItem> GetInventoryItems()
        {
            return GameWorld.GetInventoryItems();
        }

        public void EquipItemRequest(Keys requestKey)
        {
            GameWorld.EquipItemRequest(requestKey);
        }

        public void UnequipItemRequest(Keys requestKey)
        {
            GameWorld.UnequipItemRequest(requestKey);
        }

        public void DropItemRequest(Keys requestKey)
        {
            GameWorld.DropItemRequest(requestKey);
        }

        public void ApplyItemRequest(Keys requestKey)
        {
            GameWorld.ApplyItemRequest(requestKey);
        }

        public string GetSeed()
        {
            return GameWorld.Seed.ToString();
        }

        public Rectangle GetCurrentMapDimensions()
        {
            return Rectangle.WithPositionAndSize(new Point(0, 0), GameWorld.CurrentMap.Width, GameWorld.CurrentMap.Height);
        }

        public IList<IGameObject> GetLastSeenGameObjectsAtPosition(Point point)
        {
            return GameWorld.GetLastSeenGameObjectsAtPosition(point);
        }

        public IList<IGameObject> GetObjectsAt(Point point)
        {
            return GameWorld.GetObjectsAt(point);
        }

        public void UpdateFieldOfView(bool partialUpdate)
        {
            GameWorld.UpdateFieldOfView(partialUpdate);
        }

        public Path GetPathToPlayer(Point mapPosition)
        {
            return GameWorld.GetPathToPlayer(mapPosition);
        }

        public IList<MonsterStatus> GetStatusOfMonstersInView()
        {
            return GameWorld.GetStatusOfMonstersInView();
        }

        public PlayerStatus GetPlayerStatus()
        {
            return GameWorld.GetPlayerStatus();
        }

        public IList<string> GetMessagesSince(int messageLogCount)
        {
            return GameWorld.GetMessagesSince(messageLogCount);
        }

        public string GetGameObjectInformationAt(Point point)
        {
            return GameWorld.GetGameObjectInformationAt(point);
        }

        public IList<CommandResult> MoveRequest(Direction direction)
        {
            return GameWorld.MoveRequest(direction);
        }

        public IList<CommandResult> MoveRequest(Path path)
        {
            return GameWorld.MoveRequest(path);
        }

        public AutoExploreResult AutoExploreRequest()
        {
            return GameWorld.AutoExploreRequest();
        }

        public Point GetPlayerPosition()
        {
            return GameWorld.GetPlayerPosition();
        }

        public bool ExecuteNextReplayCommand()
        {
            return GameWorld.ExecuteNextReplayCommand();
        }

        public void AfterProgressiveWorldGeneration()
        {
            GameWorld.AfterProgressiveWorldGeneration();
        }

        public async Task WriteMorgueToFile(Guid gameId)
        {
            await GameWorld.WriteMorgueToFile(gameId);
        }
        
        public async Task SendMorgueToWeb(Guid gameId)
        {
            await GameWorld.SendMorgueToWeb(gameId);
        }

        public void SnapshotMorgue(string username)
        {
            GameWorld.SnapshotMorgue(username);
        }

        public Guid GetGameId()
        {
            return GameWorld.GameId;
        }

        public IList<RadioCommsItem> GetNewRadioCommsItems()
        {
            return GameWorld.GetNewRadioCommsItems();
        }

        public void LoadReplay(string gameName)
        {
            GameWorld = _gameWorldFactory.Create();
            GameWorld.LoadReplay(gameName);
        }

        public void SpawnItem(SpawnItemParams spawnItemParams)
        {
            GameWorld.SpawnItem(spawnItemParams);
        }

        public void SpawnMonster(SpawnMonsterParams spawnMonsterParams)
        {
            GameWorld.SpawnMonster(spawnMonsterParams);
        }
    }
}
