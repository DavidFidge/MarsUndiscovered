﻿using System.Text;

using BehaviourTree;
using BehaviourTree.FluentBuilder;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using GoRogue.FOV;
using GoRogue.GameFramework;
using GoRogue.Pathing;
using GoRogue.Random;

using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components.Dto;
using MarsUndiscovered.Game.Components.Factories;
using MarsUndiscovered.Game.Components.SaveData;
using MarsUndiscovered.Game.Extensions;
using MarsUndiscovered.Interfaces;

using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

using ShaiRandom.Generators;

namespace MarsUndiscovered.Game.Components
{
    public class Monster : Actor, IMementoState<MonsterSaveData>
    {
        public Breed Breed { get; set; }
        public override string Name => Breed.Name;
        public override string Description => Breed.Description;
        public override Attack MeleeAttack => Breed.MeleeAttack;
        public override Attack LineAttack => Breed.LineAttack;
        public override LightningAttack LightningAttack => Breed.LightningAttack;
        public override bool IsWallTurret => Breed.IsWallTurret;
        public int DetectionRange => Breed.DetectionRange;

        public bool FriendlyFireAllies => Breed.FriendlyFireAllies;
        public bool UseGoalMapWander { get; set; } = false;
        public MonsterState MonsterState { get; set; }

        public Monster Leader => _leader;
        
        private IFOV _fieldOfView;
        private ArrayView<SeenTile> _seenTiles;
        private ArrayView<GoalState> _goalStates;
        private GoalMap _chebyshevGoalState;
        private GoalMap _manhattanGoalState;
        private WeightedGoalMap _goalMap;
        private IBehaviour<Monster> _behaviourTree;
        private IList<BaseGameActionCommand> _nextCommands;
        private ICommandFactory _commandFactory;
        private SeenTile[] _seenTilesAfterLoad;
        private Path _wanderPath;
        private Monster _leader;
        private Path _toLeaderPath;

        public string GetInformation(Player player)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(Name);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(Description);
            stringBuilder.AppendLine();

            var percentMaxDamage = 0;
            var percentMinDamage = 0;
            var maxDamage = 0;

            if (MeleeAttack != null)
            {
                maxDamage = MeleeAttack.DamageRange.Max;
                percentMaxDamage = MeleeAttack.DamageRange.Max * 100 / player.MaxHealth;
                percentMinDamage = MeleeAttack.DamageRange.Min * 100 / player.MaxHealth;
            }
            else if (LightningAttack != null)
            {
                maxDamage = LightningAttack.Damage;
                percentMaxDamage = LightningAttack.Damage * 100 / player.MaxHealth;
                percentMinDamage = LightningAttack.Damage * 100 / player.MaxHealth;
            }

            var defeatTurns = maxDamage > 0 ? player.Health / maxDamage : 999;

            var percentText = percentMinDamage != percentMaxDamage
                ? $"between {percentMinDamage}-{percentMaxDamage}%"
                : $"{percentMinDamage}%";

            stringBuilder.AppendLine(
                $"{NameSpecificArticleUpperCase} can hit you for {percentText} of your maximum health and, at worst, could defeat you in {defeatTurns} hits."
            );

            return stringBuilder.ToString();
        }

        public Monster(IGameWorld gameWorld, uint id) : base(gameWorld, id)
        {
            _nextCommands = new List<BaseGameActionCommand>();
            CreateBehaviourTree();
        }

        public Monster WithBreed(Breed breed)
        {
            Breed = breed;
            MaxHealth = breed.MaxHealth;
            RegenRate = breed.RegenRate;
            Health = MaxHealth;

            return this;
        }

        public MonsterStatus GetMonsterStatus(Player player)
        {
            var monsterStatus = new MonsterStatus
            {
                ID = ID,
                DistanceFromPlayer = CurrentMap.DistanceMeasurement.Calculate(Position, player.Position),
                Health = Health,
                MaxHealth = MaxHealth,
                Name = Name,
                Behaviour = MonsterState.ToString().ToSeparateWords()
            };

            return monsterStatus;
        }

        public Monster AddToMap(MarsMap marsMap)
        {
            CreateGoalStates(marsMap);

            // Normally actors are not walkable as they can't be on the same square, but if an actor is on a wall it has to be walkable so that
            // it can be on the same square as a (non-walkable) wall.
            if (IsWallTurret)
                IsWalkable = true; 

            MarsGameObjectFluentExtensions.AddToMap(this, marsMap);

            _fieldOfView = new RecursiveShadowcastingFOV(marsMap.TransparencyView);
            _seenTiles = SeenTile.CreateArrayViewFromMap(marsMap);

            return this;
        }

        private void CreateGoalStates(MarsMap marsMap)
        {
            _goalStates = new ArrayView<GoalState>(marsMap.MapWidth, marsMap.MapHeight);
            _chebyshevGoalState = new GoalMap(_goalStates, Distance.Chebyshev);
            _manhattanGoalState = new GoalMap(_goalStates, Distance.Manhattan);
            _goalMap = new WeightedGoalMap(new[] { _chebyshevGoalState, _manhattanGoalState });
        }

        public void SetLoadState(IMemento<MonsterSaveData> memento)
        {
            PopulateLoadState(memento.State);
            Breed = Breed.Breeds[memento.State.BreedName];
            UseGoalMapWander = memento.State.UseGoalMapWander;
            _wanderPath = memento.State.WanderPath != null ? new Path(memento.State.WanderPath) : null;
            MonsterState = memento.State.MonsterState;
            
            if (!IsDead)
            {
                _seenTilesAfterLoad = memento.State.SeenTiles
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
            }
        }

        public IMemento<MonsterSaveData> GetSaveState()
        {
            var memento = new Memento<MonsterSaveData>(new MonsterSaveData());

            base.PopulateSaveState(memento.State);

            memento.State.BreedName = Breed.NameWithoutSpaces;
            memento.State.WanderPath = _wanderPath?.Steps.ToList();
            memento.State.UseGoalMapWander = UseGoalMapWander;
            memento.State.LeaderId = _leader?.ID;
            memento.State.MonsterState = MonsterState;

            if (!IsDead)
            {
                memento.State.SeenTiles = _seenTiles.ToArray()
                    .Select(s => s.GetSaveState())
                    .ToList();
            }

            return memento;
        }

        public override void AfterMapLoaded()
        {
            base.AfterMapLoaded();
            
            CreateGoalStates((MarsMap)CurrentMap);
            
            _seenTiles = new ArrayView<SeenTile>(_seenTilesAfterLoad, CurrentMap.Width);
            _fieldOfView = new RecursiveShadowcastingFOV(CurrentMap.TransparencyView);
        }

        public IEnumerable<BaseGameActionCommand> NextTurn(ICommandFactory commandFactory)
        {
            _commandFactory = commandFactory;
            _fieldOfView.Calculate(Position, VisualRange);
            UpdateSeenTiles(_fieldOfView.NewlySeen);
            _nextCommands.Clear();
            _behaviourTree.Tick(this);

            return _nextCommands;
        }

        private MoveCommand CreateMoveCommand(ICommandFactory commandFactory, Direction direction)
        {
            var moveCommand = commandFactory.CreateMoveCommand(GameWorld);
            moveCommand.Initialise(this, new Tuple<Point, Point>(Position, Position + direction));
            return moveCommand;
        }

        public void ResetFieldOfViewAndSeenTiles()
        {
            _fieldOfView.Reset();

            foreach (var item in _seenTiles.ToArray())
                item.HasBeenSeen = false;
        }

        private void UpdateSeenTiles(IEnumerable<Point> visiblePoints)
        {
            foreach (var point in visiblePoints)
            {
                var seenTile = _seenTiles[point];

                seenTile.HasBeenSeen = true;
            }
        }

        private void CreateBehaviourTree()
        {
            var fluentBuilder = FluentBuilder.Create<Monster>();

            _behaviourTree = fluentBuilder
                .Sequence("root")
                .Condition("map is not null", monster => CurrentMap != null)
                .Condition("on same map as player", monster => CurrentMap.Equals(GameWorld.Player.CurrentMap))
                .Selector("action selector")
                    .Subtree(MeleeAttackBehaviour())
                    .Subtree(LineAttackBehaviour())
                    .Subtree(LightningAttackBehaviour())
                    .Subtree(MoveBehavior())
                    .End()
                .End()
                .Build();
        }
        
        private IBehaviour<Monster> MoveBehavior()
        {
            var behaviour = FluentBuilder.Create<Monster>()
                .Sequence("move sequence")
                    .Condition("is not a turret", monster => !IsWallTurret)
                    .Selector("move selector")
                        .Subtree(HuntBehaviour())
                        .Subtree(FollowLeader())
                        .Subtree(WanderUsingAStarBehavior())
                        .Subtree(WanderUsingGoalMapBehavior())
                    .End()
                .End()
                .Build();

            return behaviour;
        }

        private IBehaviour<Monster> FollowLeader()
        {
            var behaviour = FluentBuilder.Create<Monster>()
                .Sequence("follow leader")
                .Condition("has leader", monster => _leader != null)
                .Condition("leader is not dead", monster => !_leader.IsDead)
                .Condition("is on same map", monster => _leader.CurrentMap.Equals(CurrentMap))
                .Condition(
                    "is not within 3 squares of leader",
                    monster =>
                    {
                        var distance = Distance.Chebyshev.Calculate(monster.Position, _leader.Position);

                        return distance > 3;
                    }
                )
                .Condition(
                    "is not blocked",
                    monster =>
                    {
                        if (_toLeaderPath == null || _leader.Position != _toLeaderPath.End ||
                            !_toLeaderPath.IsPointOnPathAndNotAtEnd(Position))
                        {
                            _toLeaderPath = CurrentMap.AStar.ShortestPath(Position, _leader.Position);
                        }

                        return _toLeaderPath != null && _toLeaderPath.Start != Point.None;
                    }
                )
                .Do(
                    "move towards leader",
                    monster =>
                    {
                        var nextPointInPath = _toLeaderPath.GetStepAfterPointWithStart(Position);

                        MonsterState = MonsterState.FollowingLeader;

                        if (!CurrentMap.GameObjectCanMove(this, nextPointInPath))
                        {
                            // Path is blocked for some reason.  Do nothing this turn.
                            _toLeaderPath = null;
                            return BehaviourStatus.Succeeded;                
                        }

                        if (nextPointInPath == Point.None)
                        {
                            _toLeaderPath = null;
                            return BehaviourStatus.Succeeded;
                        }

                        var nextDirection = Direction.GetDirection(Position, nextPointInPath);
                        
                        if (nextDirection != Direction.None)
                        {
                            var moveCommand = CreateMoveCommand(_commandFactory, nextDirection);
                            _nextCommands.Add(moveCommand);
                        }
                        
                        return BehaviourStatus.Succeeded;
                    })
                .End()
                .Build();

           return behaviour;
        }

        private IBehaviour<Monster> MeleeAttackBehaviour()
        {
            var behaviour = FluentBuilder.Create<Monster>()
                .Sequence("melee attack")
                .Condition("has melee attack", monster => MeleeAttack != null)
                .Condition("is hunting", monster => MonsterState == MonsterState.Hunting)
                .Condition("player is adjacent", monster => Position.IsNextTo(GameWorld.Player.Position, AdjacencyRule.EightWay))
                .Do(
                    "attack player",
                    monster =>
                    {
                        var attackCommand = _commandFactory.CreateMeleeAttackCommand(GameWorld);
                        attackCommand.Initialise(this, GameWorld.Player);
                        _nextCommands.Add(attackCommand);

                        return BehaviourStatus.Succeeded;
                    }
                )
                .End()
                .Build();

            return behaviour;
        }
        
        private IBehaviour<Monster> LineAttackBehaviour()
        {
            var behaviour = FluentBuilder.Create<Monster>()
                .Sequence("line attack")
                .Condition("has line attack", monster => LineAttack != null)
                .Condition("player is in field of view", monster => _fieldOfView.CurrentFOV.Contains(GameWorld.Player.Position))
                .Condition("hunting", monster => MonsterState == MonsterState.Hunting)
                .Condition("able to attack", monster =>
                {
                    var distanceToPlayer = CurrentMap.DistanceMeasurement.Calculate(Position, GameWorld.Player.Position);

                    if (distanceToPlayer > 2)
                        return false;
                    
                    if (distanceToPlayer <= 1)
                        return true;
                        
                    var directionToPlayer = Direction.GetDirection(Position, GameWorld.Player.Position);
                        
                    if (Position + directionToPlayer + directionToPlayer != GameWorld.Player.Position)
                        return false;

                    // Check for objects blocking the way between player and monster
                    if (CurrentMap.GetTerrainAt<Wall>(Position + directionToPlayer) != null)
                        return false;

                    return true;
                })
                .Do("line attack player", monster => ExecuteLineAttack(monster, GameWorld.Player)
                )
                .End()
                .Build();

            return behaviour;
        }

        private BehaviourStatus ExecuteLineAttack(Monster monster, Player gameWorldPlayer)
        {
            var directionToPlayer = Direction.GetDirection(Position, GameWorld.Player.Position);
            var targetPoint = Position + directionToPlayer + directionToPlayer;
            var lineAttackPath = Lines.GetLine(Position, targetPoint)
                .ToList();

            lineAttackPath = lineAttackPath
                .TakeWhile(p => p == Position || (CurrentMap.Contains(targetPoint) && CurrentMap.GetObjectsAt(p).All(o => o.IsGameObjectStrikeThrough())))
                .ToList();
            
            var lineAttackCommand = _commandFactory.CreateLineAttackCommand(GameWorld);
            
            lineAttackCommand.Initialise(this, lineAttackPath);
            _nextCommands.Add(lineAttackCommand);

            return BehaviourStatus.Succeeded;
        }

        private IBehaviour<Monster> LightningAttackBehaviour()
        {
            var behaviour = FluentBuilder.Create<Monster>()
                .Sequence("lightning attack")
                .Condition("has lightning attack", monster => LightningAttack != null)
                .Condition("player is in field of view", monster => _fieldOfView.CurrentFOV.Contains(GameWorld.Player.Position))
                .Condition("turret or hunting", monster => IsWallTurret || MonsterState == MonsterState.Hunting)
                .Do("lightning attack player", monster => ExecuteLightningAttack(monster, GameWorld.Player)
                )
                .End()
                .Build();

            return behaviour;
        }
        
        private BehaviourStatus ExecuteLightningAttack(Actor source, Actor target)
        {
            var lightningAttackCommand = _commandFactory.CreateLightningAttackCommand(GameWorld);
            lightningAttackCommand.Initialise(source, target.Position);

            var targets = lightningAttackCommand.GetTargets();
            if (!targets.Any(t => t is Player))
                return BehaviourStatus.Failed;

            if (!FriendlyFireAllies)
            {
                var anyAlliesAlongPath = targets.Any(t => t is Monster);

                if (anyAlliesAlongPath)
                    return BehaviourStatus.Failed;
            }

            _nextCommands.Add(lightningAttackCommand);

            return BehaviourStatus.Succeeded;
        }

        private IBehaviour<Monster> WanderUsingAStarBehavior()
        {
            var behaviour = FluentBuilder.Create<Monster>()
                .Sequence("wander using AStar sequence")
                    .Condition("can wander using AStar", monster => !monster.UseGoalMapWander)
                    .Condition(
                        "is not blocked",
                        monster =>
                        {
                            foreach (var direction in AdjacencyRule.EightWay.DirectionsOfNeighbors())
                            {
                                if (CurrentMap.GameObjectCanMove(this, Position + direction))
                                    return true;
                            }

                            // Blocked.  If player is participating in blocking, ignore this rule,
                            // it will be dealt with.
                            if (CurrentMap.DistanceMeasurement.Calculate(Position, GameWorld.Player.Position) <= 1)
                                return true;
                            
                            MonsterState = MonsterState.Idle;
                            
                            return false;
                        }
                    )
                    .Do(
                        "move to next unexplored square",
                        monster =>
                        {
                            var nextDirection = WanderUsingAStar();

                            if (nextDirection == Direction.None)
                            {
                                // move is blocked, if player is in a surrounding square, hunt immediately
                                if (CurrentMap.DistanceMeasurement.Calculate(Position, GameWorld.Player.Position) <= 1)
                                {
                                    MonsterState = MonsterState.Hunting;
                                    return BehaviourStatus.Succeeded;
                                }
                            }
                            else
                            {
                                var moveCommand = CreateMoveCommand(_commandFactory, nextDirection);
                                _nextCommands.Add(moveCommand);
                            }

                            MonsterState = MonsterState.Wandering;
                            return BehaviourStatus.Succeeded;
                        })
                .End()
                .Build();

            return behaviour;
        }
        
        private IBehaviour<Monster> WanderUsingGoalMapBehavior()
        {
            var behaviour = FluentBuilder.Create<Monster>()
                .Sequence("wander using goal map sequence")
                    .Condition("can wander using goal map", monster => monster.UseGoalMapWander)
                    .Condition(
                        "is not blocked",
                        monster =>
                        {
                            foreach (var direction in AdjacencyRule.EightWay.DirectionsOfNeighbors())
                            {
                                if (CurrentMap.GameObjectCanMove(this, Position + direction))
                                    return true;
                            }
                            
                            // Blocked.  If player is participating in blocking, ignore this rule,
                            // it will be dealt with.
                            if (CurrentMap.DistanceMeasurement.Calculate(Position, GameWorld.Player.Position) <= 1)
                                return true;

                            MonsterState = MonsterState.Idle;

                            return false;
                        }
                    )
                    .Do(
                        "move to next unexplored square",
                        monster =>
                        {
                            var nextDirection = WanderUsingGoalMap();
                            
                            if (nextDirection == Direction.None)
                                return BehaviourStatus.Failed;

                            var gameObjects = CurrentMap.GetObjectsAt<Player>(Position + nextDirection);

                            if (gameObjects.Any())
                            {
                                // Player is in the chosen square, switch to hunting but do not attack this turn
                                MonsterState = MonsterState.Hunting;
                                return BehaviourStatus.Succeeded;
                            }

                            var moveCommand = CreateMoveCommand(_commandFactory, nextDirection);
                            _nextCommands.Add(moveCommand);

                            MonsterState = MonsterState.Wandering;
                            return BehaviourStatus.Succeeded;
                        }
                    )
                .End()
                .Build();

            return behaviour;
        }
        
        private IBehaviour<Monster> HuntBehaviour()
        {
            var behaviour = FluentBuilder.Create<Monster>()
                .Sequence("hunt")
                    .Condition("player in field of view", monster => _fieldOfView.CurrentFOV.Contains(GameWorld.Player.Position))
                    .Condition("player has been detected", monster =>
                    {
                        if (MonsterState == MonsterState.Hunting)
                            return true;
                        
                        var distance = CurrentMap.DistanceMeasurement.Calculate(Position, GameWorld.Player.Position);
                        
                        if (distance <= DetectionRange)
                        {
                            if (GlobalRandom.DefaultRNG.NextInt(5) == 0)
                                MonsterState = MonsterState.Hunting;
                        }

                        return MonsterState == MonsterState.Hunting;
                    })
                    .Do(
                        "move towards player",
                        monster =>
                        {
                            var nextDirection = Hunt();

                            if (nextDirection == Direction.None)
                                return BehaviourStatus.Failed;

                            var actorsAtPosition = CurrentMap.GetObjectsAt<Actor>(Position + nextDirection);

                            if (actorsAtPosition.Any())
                            {
                                // If we entered here then the monster would have switched to hunting while
                                // adjacent to the player.  Do not move anywhere - next turn the monster
                                // should perform an attack.
                                return BehaviourStatus.Succeeded;
                            }

                            var moveCommand = CreateMoveCommand(_commandFactory, nextDirection);
                            _nextCommands.Add(moveCommand);
                         
                            return BehaviourStatus.Succeeded;
                        }
                    )
                .End()
                .Build();

            return behaviour;
        }

        public Direction Hunt()
        {
            _goalStates.Clear();

            // Hunt radius only requires the distance between player and monster, plus an extra 3 for monsters to be able to surround the player
            // or walk around any blocking enemies. This may limit the monster's ability to walk around blocking terrain if the path around
            // the terrain is not included in this submap, but 99% of cases should be fine, and may sometimes be beneficial as it means
            // monsters can't be tricked into taking a longer path to the player.
            var range = Rectangle.GetIntersection(CurrentMap.RectangleCoveringPoints(GameWorld.Player.Position, Position).Expand(3, 3), CurrentMap.Bounds());
            
            for (var x = range.MinExtentX; x <= range.MaxExtentX; x++)
            {
                for (var y = range.MinExtentY; y <= range.MaxExtentY; y++)
                {
                    var gameObjects = CurrentMap.GetObjectsAt(x, y);

                    _goalStates[x, y] = GoalState.Clear;

                    foreach (var gameObject in gameObjects)
                    {
                        if (gameObject is Player _)
                        {
                            _goalStates[x, y] = GoalState.Goal;
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

            _chebyshevGoalState.Update(range);
            _manhattanGoalState.Update(range);

            return _goalMap.GetDirectionOfMinValue(Position, AdjacencyRule.EightWay, false);
        }

        public Direction WanderUsingGoalMap()
        {
            _goalStates.Clear();

            var range = CurrentMap.Bounds();

            for (var x = range.MinExtentX; x <= range.MaxExtentX; x++)
            {
                for (var y = range.MinExtentY; y <= range.MaxExtentY; y++)
                {
                    if (!_seenTiles[x, y].HasBeenSeen)
                    {
                        // This can include walls, so if dumping a gaol map to text you may see walls come up with a goal of 0 until they come into the field of view 
                        _goalStates[x, y] = GoalState.Goal;
                        continue;
                    }
                    
                    var gameObjects = CurrentMap.GetObjectsAt(x, y);

                    _goalStates[x, y] = GoalState.Clear;

                    foreach (var gameObject in gameObjects)
                    {
                        if (gameObject.IsGameObjectObstacle())
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

        public Direction WanderUsingAStar()
        {
            var currentPathLocation = Point.None;

            if (_wanderPath.IsPointOnPathAndNotAtEnd(Position))
                currentPathLocation = Position;

            if (currentPathLocation == Point.None)
            {
                _wanderPath = GetNewWanderPath();
                
                if (_wanderPath != null)
                    currentPathLocation = _wanderPath.Start;
                
                // No floor tiles are left unexplored OR no non-blocking path could be found. Wandering has a condition where it ensures the entity is not completely blocked before entering this method, so 
                // it is likely that the map has been split by blocking monsters or by temporary walls.  It makes the most sense here to reset the field of view.
                if (currentPathLocation == Point.None)
                {
                    ResetFieldOfViewAndSeenTiles();
                    return Direction.None;
                }
            }
            
            System.Diagnostics.Debug.Assert(currentPathLocation != Point.None);

            var nextPointInPath = _wanderPath.GetStepAfterPointWithStart(currentPathLocation);

            if (!CurrentMap.GameObjectCanMove(this, nextPointInPath))
            {
                // Path is blocked. Stay stationary this turn and find a new path next turn.
                _wanderPath = null;
                ResetFieldOfViewAndSeenTiles();
                return Direction.None;                
            }

            if (nextPointInPath == Point.None)
            {
                System.Diagnostics.Debug.Fail("Could not find a next point in path and path lookup logic did not return Direction.None. This should never happen.");
                return Direction.None;
            }

            return Direction.GetDirection(Position, nextPointInPath);
        }

        private Path GetNewWanderPath()
        {
            Path wanderPath = null;

            var findPathToUnseenFloorTileTries = 20;
            var checkedIndexes = new List<int>(20);

            for (var i = 0; i < findPathToUnseenFloorTileTries; i++)
            {
                var randomUnseenTileIndex = GetRandomUnseenFloorTileIndex(checkedIndexes);
                
                if (randomUnseenTileIndex == -1)
                {
                    return null;
                }

                wanderPath = CurrentMap.AStar.ShortestPath(Position, Point.FromIndex(randomUnseenTileIndex, CurrentMap.Width));

                if (wanderPath != null)
                    break;

                // Found point is blocked. Ignore it on subsequent executions
                checkedIndexes.Add(i);
            }

            return wanderPath;
        }

        private int GetRandomUnseenFloorTileIndex(List<int> checkedIndexes)
        {
            var randomUnseenTileIndex = GlobalRandom.DefaultRNG.RandomPosition(_seenTiles).ToIndex(_seenTiles.Width);

            var foundUnseenTile = false;

            for (var i = randomUnseenTileIndex; i < _seenTiles.Count; i++)
            {
                if (checkedIndexes.Contains(i))
                    continue;
                
                if (!_seenTiles[i].HasBeenSeen && CurrentMap.GetTerrainAt(Point.FromIndex(i, _seenTiles.Width)) is Floor)
                {
                    randomUnseenTileIndex = i;
                    foundUnseenTile = true;
                    break;
                }
            }

            if (!foundUnseenTile)
            {
                for (var i = 0; i < randomUnseenTileIndex; i++)
                {
                    if (checkedIndexes.Contains(i))
                        continue;
                    
                    if (!_seenTiles[i].HasBeenSeen && CurrentMap.GetTerrainAt(Point.FromIndex(i, _seenTiles.Width)) is Floor)
                    {
                        randomUnseenTileIndex = i;
                        foundUnseenTile = true;
                        break;
                    }
                }
            }

            if (!foundUnseenTile)
                return -1;

            return randomUnseenTileIndex;
        }

        public IGridView<double?> GetGoalMap()
        {
            return _goalMap;
        }

        public Path GetWanderPath()
        {
            return _wanderPath;
        }

        public void SetLeader(Monster monster)
        {
            _leader = monster;
        }
    }
}