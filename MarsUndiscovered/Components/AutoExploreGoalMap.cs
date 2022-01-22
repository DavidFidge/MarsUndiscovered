using System.Linq;

using GoRogue.Pathing;

using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Components
{
    public class AutoExploreGoalMap
    {
        private ArrayView<GoalState> _goalStates;
        public GoalMap GoalMap { get; set; }
        private int _lastGameTurn = -1;

        public void Rebuild(GameWorld gameWorld)
        {
            // No need to rebuild if there's been no turns since last rebuild
            if (gameWorld.GameTurnService.TurnNumber == _lastGameTurn)
                return;

            _lastGameTurn = gameWorld.GameTurnService.TurnNumber;

            var map = gameWorld.CurrentMap;
            var seenTiles = gameWorld.CurrentMap.SeenTiles;

            _goalStates = new ArrayView<GoalState>(map.Width, map.Height);

            // If a monster is directly around the player then attack it
            var monstersAroundPlayer = map.GetObjectsAround<Monster>(gameWorld.Player.Position, AdjacencyRule.EightWay);

            if (monstersAroundPlayer.Any())
            {
                _goalStates[monstersAroundPlayer.First().Position] = GoalState.Goal;
            }
            else
            {
                var hasGoalBeenSet = false;

                for (var x = 0; x < map.Width; x++)
                {
                    for (var y = 0; y < map.Height; y++)
                    {
                        if (!seenTiles[x, y].HasBeenSeen)
                        {
                            _goalStates[x, y] = GoalState.Goal;
                            hasGoalBeenSet = true;
                            continue;
                        }

                        if (!map.PlayerFOV.BooleanResultView[x, y])
                        {
                            if (seenTiles[x, y].HasUndroppedItem)
                            {
                                _goalStates[x, y] = GoalState.Goal;
                                hasGoalBeenSet = true;
                                continue;
                            }
                        }

                        if (map.PlayerFOV.BooleanResultView[x, y])
                        {
                            if (map.GetObjectAt<Item>(x, y) != null)
                            {
                                _goalStates[x, y] = GoalState.Goal;
                                hasGoalBeenSet = true;
                                continue;
                            }
                        }

                        var gameObjects = map
                            .GetObjectsAt(x, y)
                            .ToList();

                        _goalStates[x, y] = GoalState.Clear;

                        foreach (var gameObject in gameObjects)
                        {
                            if (gameObject is Player _)
                            {
                                _goalStates[x, y] = GoalState.Obstacle;
                                break;
                            }

                            if (gameObject is Indestructible _)
                            {
                                _goalStates[x, y] = GoalState.Obstacle;
                                break;
                            }

                            if (gameObject is Wall _)
                            {
                                _goalStates[x, y] = GoalState.Obstacle;
                                break;
                            }

                            // We can reach here if the object type was an item or a floor, if so keep on looking.
                        }
                    }
                }

                // If no goal has yet been set then set the goal to the map exit
                if (!hasGoalBeenSet)
                {
                    var mapExitDown = gameWorld.MapExits.Values
                        .Where(m => m.CurrentMap.Equals(map))
                        .Where(m => m.Direction == Direction.Down)
                        .ToList();

                    if (mapExitDown.Any())
                        _goalStates[mapExitDown.First().Position] = GoalState.Goal;
                }
            }

            GoalMap = new GoalMap(_goalStates, Distance.Chebyshev);
        }
    }
}