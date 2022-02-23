using System;
using System.Collections.Generic;
using System.Linq;

using BehaviourTree;
using BehaviourTree.FluentBuilder;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;
using GoRogue.FOV;
using GoRogue.GameFramework;
using GoRogue.Pathing;

using MarsUndiscovered.Components.SaveData;
using MarsUndiscovered.Interfaces;

using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Components
{
    public class MonsterGoal : IMementoState<MonsterGoalSaveData>
    {
        private Direction _nextDirection;
        private IFOV _fieldOfView;
        private ArrayView<SeenTile> _seenTiles;

        private Monster _monster;
        private MarsMap Map => (MarsMap)_monster.CurrentMap;
        private IGameWorld GameWorld => _monster.GameWorld;
        private Point Position => _monster.Position;

        private ArrayView<GoalState> _goalStates;
        private GoalMap _chebyshevGoalState;
        private GoalMap _manhattanGoalState;
        private WeightedGoalMap _goalMap;
        private IBehaviour<MonsterGoal> _behaviourTree;

        public MonsterGoal(Monster monster)
        {
            _monster = monster;

            _goalStates = new ArrayView<GoalState>(MarsMap.MapWidth, MarsMap.MapHeight);
            _chebyshevGoalState = new GoalMap(_goalStates, Distance.Chebyshev);
            _manhattanGoalState = new GoalMap(_goalStates, Distance.Manhattan);
            _goalMap = new WeightedGoalMap(new[] { _chebyshevGoalState, _manhattanGoalState });
        }

        public void Initialise()
        {
            _fieldOfView = new RecursiveShadowcastingFOV(Map.TransparencyView);
            _seenTiles = SeenTile.CreateArrayViewFromMap(Map);
            
            var fluentBuilder = FluentBuilder.Create<MonsterGoal>();

            _behaviourTree = fluentBuilder
                .Sequence("root")
                    .Selector("move selector")
                        .Sequence("hunt")
                            .Condition("on same map as player", monsterGoal => Map.Equals(GameWorld.Player.CurrentMap))
                            .Condition("player in field of view", monsterGoal => _fieldOfView.CurrentFOV.Contains(GameWorld.Player.Position))
                            .Do("move towards player",
                            monsterGoal =>
                                {
                                    _nextDirection = Hunt();
                                    return BehaviourStatus.Succeeded;
                                })
                            .End()
                        .Selector("wander")
                            .Do("move to next unexplored square",
                            monsterGoal =>
                            {
                                _nextDirection = Wander();

                                if (_nextDirection == Direction.None)
                                    return BehaviourStatus.Failed;

                                return BehaviourStatus.Succeeded;
                            })
                           .Sequence("rebuild field of view sequence")
                               .Condition("is blocked",
                                monsterGoal =>
                                {
                                    foreach (var direction in AdjacencyRule.EightWay.DirectionsOfNeighbors())
                                    {
                                        if (Map.GameObjectCanMove(_monster, Position + direction))
                                            return false;
                                    }

                                    return true;
                                })
                                .Do("Rebuild fieldOfView", monsterGoal =>
                                {
                                    ResetFieldOfViewAndSeenTiles();
                                    return BehaviourStatus.Succeeded;
                                })
                            .End()
                        .End()
                    .End()
                .End()
                .Build();
        }

        public void ResetFieldOfViewAndSeenTiles()
        {
            _fieldOfView.Reset();
            _fieldOfView.Calculate(Position);

            foreach (var item in _seenTiles.ToArray())
                item.HasBeenSeen = false;

            UpdateSeenTiles(_fieldOfView.CurrentFOV);
        }

        public Direction GetNextMove(IGameWorld gameWorld)
        {
            _behaviourTree.Tick(this);

            _fieldOfView.Calculate(Position);

            UpdateSeenTiles(_fieldOfView.NewlySeen);

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

        public IMemento<MonsterGoalSaveData> GetSaveState()
        {
            var memento = new Memento<MonsterGoalSaveData>();

            memento.State.MonsterId = _monster.ID;
            memento.State.SeenTiles = _seenTiles.ToArray()
                .Select(s => s.GetSaveState())
                .ToList();

            return memento;
        }

        public void SetLoadState(IMemento<MonsterGoalSaveData> memento)
        {
            var seenTiles = memento.State.SeenTiles
                .Select(s =>
                    {
                        var seenTiles = new SeenTile(s.State.Point);

                        // Monster goals currently don't care about last seen game objects
                        // so we can pass in a new dictionary
                        seenTiles.SetLoadState(s, new Dictionary<uint, IGameObject>());
                        return seenTiles;
                    }
                )
                .ToArray();

            _seenTiles = new ArrayView<SeenTile>(seenTiles, MarsMap.MapWidth);

            _fieldOfView.Calculate(Position);
        }

        public Direction Hunt()
        {
            _goalStates.Clear();

            for (var x = 0; x < Map.Width; x++)
            {
                for (var y = 0; y < Map.Height; y++)
                {
                    var gameObjects = Map
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

            return _goalMap.GetDirectionOfMinValue(Position, AdjacencyRule.EightWay, false);
        }

        public Direction Wander()
        {
            _goalStates.Clear();

            for (var x = 0; x < MarsMap.MapWidth; x++)
            {
                for (var y = 0; y < MarsMap.MapHeight; y++)
                {
                    if (!_seenTiles[x, y].HasBeenSeen)
                    {
                        _goalStates[x, y] = GoalState.Goal;
                        continue;
                    }

                    var gameObjects = Map
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

            _chebyshevGoalState.Update();
            _manhattanGoalState.Update();

            return _goalMap.GetDirectionOfMinValue(Position, AdjacencyRule.EightWay, false);
        }
    }
}