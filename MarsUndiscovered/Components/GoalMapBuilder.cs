using System.Linq;
using GoRogue.Pathing;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Components
{
    public class GoalMapBuilder
    {
        public GoalMap GoalMap { get; set; }
        public ArrayView<GoalState> GoalStates { get; set; }
        public FleeMap FleeMap { get; set; }

        public static GoalMapBuilder Build(GameWorld gameWorld)
        {
            var goalMapBuilder = new GoalMapBuilder();

            goalMapBuilder.Rebuild(gameWorld);

            return goalMapBuilder;
        }

        public void Rebuild(GameWorld gameWorld)
        {
            GoalStates = new ArrayView<GoalState>(gameWorld.Map.Width, gameWorld.Map.Height);

            for (var x = 0; x < gameWorld.Map.Width; x++)
            {
                for (var y = 0; y < gameWorld.Map.Height; y++)
                {
                    var gameObjects = gameWorld.Map
                        .GetObjectsAt(new Point(x, y))
                        .Reverse()
                        .ToList();

                    foreach (var gameObject in gameObjects)
                    {
                        switch (gameObject)
                        {
                            case Player _:
                                GoalStates[x, y] = GoalState.Goal;
                                continue; // always continue for player

                            case Monster _:
                                GoalStates[x, y] = GoalState.Obstacle;
                                continue; // always continue for monster

                            case Wall _:
                                GoalStates[x, y] = GoalState.Obstacle;
                                break;

                            case Floor _:
                                GoalStates[x, y] = GoalState.Clear;
                                break;
                        }
                    }
                }
            }

            GoalMap = new GoalMap(GoalStates, Distance.Chebyshev);
            FleeMap = new FleeMap(GoalMap);
        }

        public void UpdateForMonsterMove(Point from, Point to)
        {
            GoalStates[from] = GoalState.Clear;
            GoalStates[to] = GoalState.Obstacle;

            GoalMap.Update();
            FleeMap = new FleeMap(GoalMap);
        }

        public void UpdateForPlayerMove(Point from, Point to)
        {
            // Swap via deconstruction
            (GoalStates[@from], GoalStates[to]) = (GoalStates[to], GoalStates[@from]);

            GoalMap.UpdatePathsOnly();
            FleeMap = new FleeMap(GoalMap);
        }
    }
}