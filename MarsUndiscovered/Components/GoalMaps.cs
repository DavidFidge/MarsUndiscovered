using System.Linq;
using FrigidRogue.MonoGame.Core.Components;
using GoRogue.Pathing;

using MarsUndiscovered.Interfaces;
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

        public void Rebuild(IGameWorld gameWorld)
        {
            _goalStates = new ArrayView<GoalState>(gameWorld.Map.Width, gameWorld.Map.Height);

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
                                _goalStates[x, y] = GoalState.Goal;
                                continue; // always continue for player

                            case Monster _:
                                _goalStates[x, y] = GoalState.Obstacle;
                                continue; // always continue for monster

                            case Wall _:
                                _goalStates[x, y] = GoalState.Obstacle;
                                break;

                            case Floor _:
                                _goalStates[x, y] = GoalState.Clear;
                                break;
                        }
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