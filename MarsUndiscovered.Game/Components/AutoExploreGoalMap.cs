using GoRogue.Pathing;
using MarsUndiscovered.Game.Extensions;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Game.Components
{
    public class AutoExploreGoalMap
    {
        public IGridView<double?> GoalMap => _goalMap;

        private ArrayView<GoalState> _goalStates;
        private GoalMap _goalMap;
        private int _lastGameTurn = -1;
        private int _mapWidth;
        private int _mapHeight;

        public void Rebuild(GameWorld gameWorld, bool fallbackToMapExit = false)
        {
            // No need to rebuild if there's been no turns since last rebuild
            if (gameWorld.GameTurnService.TurnNumber == _lastGameTurn)
                return;

            if (_mapWidth != gameWorld.CurrentMap.MapWidth || _mapHeight != gameWorld.CurrentMap.MapHeight)
            {
                _mapWidth = gameWorld.CurrentMap.MapWidth;
                _mapHeight = gameWorld.CurrentMap.MapHeight;

                _goalStates = new ArrayView<GoalState>(_mapWidth, _mapHeight);
                _goalMap = new GoalMap(_goalStates, Distance.Chebyshev);
            }
            else
            {
                _goalStates.Clear();
            }

            _lastGameTurn = gameWorld.GameTurnService.TurnNumber;

            var map = gameWorld.CurrentMap;
            var seenTiles = gameWorld.CurrentMap.SeenTiles;

            // If a monster is directly around the player then attack it
            var monstersAroundPlayer = map.GetObjectsAround<Monster>(gameWorld.Player.Position, AdjacencyRule.EightWay);

            if (monstersAroundPlayer.Any())
            {
                _goalStates[monstersAroundPlayer.First().Position] = GoalState.Goal;
            }
            else
            {
                for (var x = 0; x < map.Width; x++)
                {
                    for (var y = 0; y < map.Height; y++)
                    {
                        if (!seenTiles[x, y].HasBeenSeen)
                        {
                            _goalStates[x, y] = GoalState.Goal;
                            continue;
                        }

                        if (!map.PlayerFOV.BooleanResultView[x, y])
                        {
                            if (seenTiles[x, y].HasUndroppedItem)
                            {
                                _goalStates[x, y] = GoalState.Goal;
                                continue;
                            }
                        }

                        if (map.PlayerFOV.BooleanResultView[x, y])
                        {
                            var item = map.GetObjectAt<Item>(x, y);

                            if (item is { HasBeenDropped: false })
                            {
                                _goalStates[x, y] = GoalState.Goal;
                                continue;
                            }
                        }

                        var gameObjects = map.GetObjectsAt(x, y);

                        _goalStates[x, y] = GoalState.Clear;

                        foreach (var gameObject in gameObjects)
                        {
                            if (gameObject is Player _)
                            {
                                _goalStates[x, y] = GoalState.Clear;
                                break;
                            }

                            if (gameObject.IsGameObjectObstacle())
                            {
                                _goalStates[x, y] = GoalState.Obstacle;
                                break;
                            }

                            // We can reach here if the object type was an item or a floor, if so keep on looking.
                        }
                    }
                }
            }

            _goalMap.Update();

            if (fallbackToMapExit)
            {
                var tryNextMove = _goalMap.GetDirectionOfMinValue(gameWorld.Player.Position, false);

                if (tryNextMove == Direction.None)
                {
                    var mapExitDown = gameWorld.MapExits.Values
                        .Where(m => m.CurrentMap.Equals(map))
                        .Where(m => m.Direction == Direction.Down)
                        .ToList();

                    if (mapExitDown.Any())
                    {
                        _goalStates[mapExitDown.First().Position] = GoalState.Goal;
                        _goalMap.Update();
                    }
                }
            }
        }
    }
}
