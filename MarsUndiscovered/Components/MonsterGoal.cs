using System;
using System.Collections.Generic;
using System.Linq;

using Assimp.Unmanaged;

using AutoMapper;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;

using GoRogue.FOV;
using GoRogue.Pathing;

using MarsUndiscovered.Components.SaveData;
using MarsUndiscovered.Interfaces;

using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

using SharpDX.MediaFoundation;

namespace MarsUndiscovered.Components
{
    public class MonsterGoal : IMementoState<MonsterGoalSaveData>
    {
        private Direction _nextDirection;
        private IFOV _fieldOfView;
        private ArrayView<SeenTile> _seenTiles;

        private State<MonsterGoal> _currentState;
        private WanderState _wanderState;
        private HuntPlayerState _huntPlayerState;
        private Monster _monster;
        private MarsMap Map => (MarsMap)_monster.CurrentMap;
        private Point Position => _monster.Position;
        public State<MonsterGoal> CurrentState => _currentState;

        public MonsterGoal(Monster monster)
        {
            _monster = monster;
            _fieldOfView = new RecursiveShadowcastingFOV(Map.TransparencyView);
            _seenTiles = new ArrayView<SeenTile>(Map.Width, Map.Height);
            _wanderState = new WanderState();
            _huntPlayerState = new HuntPlayerState();
            _currentState = _wanderState;
        }

        public void ChangeState(State<MonsterGoal> newState)
        {
            _currentState?.Exit(this);
            _currentState = newState;
            _currentState.Enter(this);
        }

        public void ResetFieldOfViewAndSeenTiles()
        {
            _fieldOfView.Reset();
            _fieldOfView.Calculate(Position);

            _fieldOfView = new RecursiveShadowcastingFOV(Map.TransparencyView);
            _fieldOfView.Calculate(Position);

            foreach (var item in _seenTiles.ToArray())
                item.HasBeenSeen = false;

            UpdateSeenTiles(_fieldOfView.CurrentFOV);
        }

        public Direction GetNextMove(IGameWorld gameWorld)
        {
            _currentState.Execute(this);

            _fieldOfView.Calculate(Position);

            UpdateSeenTiles(_fieldOfView.NewlySeen);

            var seenPlayer = false;

            foreach (var point in _fieldOfView.NewlySeen)
            {
                if (gameWorld.Player.Position.Equals(point))
                {
                    ChangeState(_huntPlayerState);
                    seenPlayer = true;
                }
            }

            if (!seenPlayer)
            {
                ChangeState(_wanderState);
            }

            return _nextDirection;
        }

        private void UpdateSeenTiles(IEnumerable<Point> visiblePoints)
        {
            foreach (var point in visiblePoints)
            {
                var seenTile = _seenTiles[point];

                seenTile.HasBeenSeen = true;
            }
        }

        public IMemento<MonsterGoalSaveData> GetSaveState(IMapper mapper)
        {
            throw new System.NotImplementedException();
        }

        public void SetLoadState(IMemento<MonsterGoalSaveData> memento, IMapper mapper)
        {
            throw new System.NotImplementedException();
        }

        public class WanderState : State<MonsterGoal>
        {
            private readonly ArrayView<GoalState> _goalStates;
            private readonly GoalMap _goalMap;

            public WanderState()
            {
                _goalStates = new ArrayView<GoalState>(MarsMap.MapWidth, MarsMap.MapHeight);
                _goalMap = new GoalMap(_goalStates, Distance.Chebyshev);
            }

            public override void Enter(MonsterGoal monsterGoal)
            {
            }

            public override void Execute(MonsterGoal monsterGoal)
            {
                _goalStates.Clear();

                for (var x = 0; x < MarsMap.MapWidth; x++)
                {
                    for (var y = 0; y < MarsMap.MapHeight; y++)
                    {
                        if (!monsterGoal._seenTiles[x, y].HasBeenSeen)
                        {
                            _goalStates[x, y] = GoalState.Goal;
                            continue;
                        }

                        var gameObjects = monsterGoal.Map
                            .GetObjectsAt(x, y)
                            .ToList();

                        _goalStates[x, y] = GoalState.Clear;

                        foreach (var gameObject in gameObjects)
                        {
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
                        }
                    }
                }

                _goalMap.Update();


                monsterGoal._nextDirection = _goalMap.GetDirectionOfMinValue(monsterGoal.Position, AdjacencyRule.EightWay, false);

                if (monsterGoal._nextDirection == Direction.None)
                {
                    // Monster cannot move for some reason. See if it is blocked by other monsters. If not, it is likely the
                    // monster has completely explored the map. Reset its seen tiles and field of view so that it will 
                    // start exploring again.
                    var actors = monsterGoal.Map.GetObjectsAround<Actor>(monsterGoal.Position, AdjacencyRule.EightWay);

                    if (!actors.Any())
                        monsterGoal.ResetFieldOfViewAndSeenTiles();
                }
            }

            public override void Exit(MonsterGoal monsterGoal)
            {
            }

            public override void Reset(MonsterGoal monsterGoal)
            {
            }
        }

        public class HuntPlayerState : State<MonsterGoal>
        {
            private readonly ArrayView<GoalState> _goalStates;
            private readonly GoalMap _chebyshevGoalState;
            private readonly GoalMap _manhattanGoalState;
            private readonly WeightedGoalMap _goalMap;

            public HuntPlayerState()
            {
                _goalStates = new ArrayView<GoalState>(MarsMap.MapWidth, MarsMap.MapHeight);
                _chebyshevGoalState = new GoalMap(_goalStates, Distance.Chebyshev);
                _manhattanGoalState = new GoalMap(_goalStates, Distance.Manhattan);
                _goalMap = new WeightedGoalMap(new[] { _chebyshevGoalState, _manhattanGoalState });
            }

            public override void Enter(MonsterGoal monsterGoal)
            {
            }

            public override void Execute(MonsterGoal monsterGoal)
            {
                _goalStates.Clear();

                for (var x = 0; x < monsterGoal.Map.Width; x++)
                {
                    for (var y = 0; y < monsterGoal.Map.Height; y++)
                    {
                        var gameObjects = monsterGoal.Map
                            .GetObjectsAt(x, y)
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

                _chebyshevGoalState.Update();
                _manhattanGoalState.Update();

                monsterGoal._nextDirection = _goalMap.GetDirectionOfMinValue(monsterGoal.Position, AdjacencyRule.EightWay, false);
            }

            public override void Exit(MonsterGoal monsterGoal)
            {
            }

            public override void Reset(MonsterGoal monsterGoal)
            {
            }
        }
    }
}