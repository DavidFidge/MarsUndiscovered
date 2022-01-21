using System.Linq;
using FrigidRogue.MonoGame.Core.Components;
using GoRogue.Pathing;

using MarsUndiscovered.Messages;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Components
{
    public class GoalMaps : BaseComponent
    {
        private GoalMap _chebyshevGoalState;
        private GoalMap _manhattanGoalState;
        private ArrayView<GoalState> _goalStates;
        public WeightedGoalMap GoalMap { get; set; }

        public void Rebuild(MarsMap map)
        {
            _goalStates = new ArrayView<GoalState>(map.Width, map.Height);

            for (var x = 0; x < map.Width; x++)
            {
                for (var y = 0; y < map.Height; y++)
                {
                    var gameObjects = map
                        .GetObjectsAt(new Point(x, y))
                        .Reverse()
                        .ToList();

                    _goalStates[x, y] = GoalState.Clear;

                    foreach (var gameObject in gameObjects)
                    {
                        if (gameObject is Player _)
                        {
                            _goalStates[x, y] = GoalState.Goal;
                            break;
                        }
                        if (gameObject is Monster _)
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

            _chebyshevGoalState = new GoalMap(_goalStates, Distance.Chebyshev);
            _manhattanGoalState = new GoalMap(_goalStates, Distance.Manhattan);

            GoalMap = new WeightedGoalMap(new[] { _chebyshevGoalState, _manhattanGoalState });

            Mediator.Publish(new GoalMapChangedNotification());
        }

        public void UpdateForMonsterMove(Point from, Point to)
        {
            _goalStates[from] = GoalState.Clear;
            _goalStates[to] = GoalState.Obstacle;

            _chebyshevGoalState.Update();
            _manhattanGoalState.Update();

            GoalMap = new WeightedGoalMap(new[] { _chebyshevGoalState, _manhattanGoalState });

            Mediator.Publish(new GoalMapChangedNotification());
        }

        public void UpdateForPlayerMove(Point from, Point to)
        {
            // Swap via deconstruction
            (_goalStates[@from], _goalStates[to]) = (_goalStates[to], _goalStates[@from]);

            _chebyshevGoalState.UpdatePathsOnly();
            _manhattanGoalState.UpdatePathsOnly();

            GoalMap = new WeightedGoalMap(new[] { _chebyshevGoalState, _manhattanGoalState });

            Mediator.Publish(new GoalMapChangedNotification());
        }
    }
}