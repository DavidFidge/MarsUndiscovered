using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using BehaviourTree;
using BehaviourTree.FluentBuilder;
using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using GoRogue;
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
        public override char AsciiCharacter => Breed.AsciiCharacter;

        public Breed Breed { get; set; }
        public override string Name => Breed.Name;
        public override string Description => Breed.Description;
        public override Attack MeleeAttack => Breed.MeleeAttack;
        public override Attack LineAttack => Breed.LineAttack;
        public override LightningAttack LightningAttack => Breed.LightningAttack;
        public override bool IsWallTurret => Breed.IsWallTurret;
        public int DetectionRange => Breed.DetectionRange;

        public bool FriendlyFireAllies => Breed.FriendlyFireAllies;
        public bool UseGoalMapWander { get; set; }

        public bool CanBeConcussed => Breed.WeaknessToConcuss;
        
        public bool IsConcussed { get; set; }
        
        public int SearchCooldown { get; set; }

        public MonsterState MonsterState { get; set; }

        public Actor Leader { get; set; }
        public Actor Target { get; set; }

        // TODO include in save game
        public Actor TargetOutOfFov { get; set; }

        private IFOV _fieldOfView;
        private ArrayView<SeenTile> _seenTiles;
        private ArrayView<GoalState> _goalStates;
        private GoalMap _chebyshevGoalState;
        private GoalMap _manhattanGoalState;
        private WeightedGoalMap _goalMap;
        private IBehaviour<Monster> _behaviourTree;
        private IList<BaseGameActionCommand> _nextCommands;
        private ICommandCollection _commandFactory => GameWorld.CommandCollection;
        private SeenTile[] _seenTilesAfterLoad;
        private Path _wanderPath;

        private Path _toLeaderPath;

        public string GetInformation(Player player)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"I see a {Name}.");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"My scanner says: \"{Description}\"");
            stringBuilder.AppendLine();

            var maxDamage = 0;
            
            if (MeleeAttack != null)
            {
                maxDamage = MeleeAttack.DamageRange.Max;
            }
            else if (LightningAttack != null)
            {
                maxDamage = LightningAttack.Damage;
            }
            
            var defeatTurns = maxDamage > 0 ? player.Health / maxDamage : 999;

            var dangerText = "harmless";

            if (defeatTurns <= 1)
                dangerText = "deadly";
            else if (defeatTurns <= 3)
                dangerText = "dangerous";
            else if (defeatTurns <= 5)
                dangerText = "strong";
            else if (defeatTurns <= 10)
                dangerText = "weak";

            stringBuilder.AppendLine(
                $"{GetSentenceName(false, false)} looks {dangerText}."
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
                Behaviour = MonsterState.ToString().ToSeparateWords(),
                Position = Position,
                SearchCooldown = SearchCooldown,
                CanBeConcussed = CanBeConcussed,
                IsConcussed = IsConcussed
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

            PopulateSaveState(memento.State);

            memento.State.BreedName = Breed.NameWithoutSpaces;
            memento.State.WanderPath = _wanderPath?.Steps.ToList();
            memento.State.UseGoalMapWander = UseGoalMapWander;
            memento.State.LeaderId = Leader?.ID;
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

        public IEnumerable<BaseGameActionCommand> NextTurn()
        {
            _fieldOfView.Calculate(Position, VisualRange);
            UpdateSeenTiles(_fieldOfView.NewlySeen);
            _nextCommands.Clear();
            _behaviourTree.Tick(this);

            return _nextCommands;
        }

        private MoveCommand CreateMoveCommand(Direction direction)
        {
            var moveCommand = _commandFactory.CreateCommand<MoveCommand>(GameWorld);
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
                    .Selector("pick target")
                        .AlwaysSucceed("pick target always succeed") // allows root sequence to continue
                            .Subtree(PickTargetBehavior())
                            .End()
                        .End()
                    .Selector("action selector")
                        .Subtree(ConcussBehaviour())
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
                        .Subtree(SearchingBehavior())
                        .Subtree(HuntBehaviour())
                        //// Try search again after hunt, hunt can transition to search if the target is not in FOV.
                        .Subtree(SearchingBehavior())
                        .Subtree(FollowLeader())
                        .Subtree(WanderBehavior())
                    .End()
                .End()
                .Build();

            return behaviour;
        }

        private IBehaviour<Monster> FollowLeader()
        {
            var behaviour = FluentBuilder.Create<Monster>()
                .Sequence("follow leader")
                .Condition("has leader", monster => Leader != null)
                .Condition("leader is not dead", monster => !Leader.IsDead)
                .Condition("is on same map", monster => Leader.CurrentMap.Equals(CurrentMap))
                .Condition(
                    "is not within 3 squares of leader",
                    monster =>
                    {
                        var distance = Distance.Chebyshev.Calculate(monster.Position, Leader.Position);

                        return distance > 3;
                    }
                )
                .Condition(
                    "is not blocked",
                    monster =>
                    {
                        if (_toLeaderPath == null || Leader.Position != _toLeaderPath.End ||
                            !_toLeaderPath.IsPointOnPathAndNotAtEnd(Position))
                        {
                            _toLeaderPath = CurrentMap.AStar.ShortestPath(Position, Leader.Position);
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
                            var moveCommand = CreateMoveCommand(nextDirection);
                            _nextCommands.Add(moveCommand);
                        }
                        
                        return BehaviourStatus.Succeeded;
                    })
                .End()
                .Build();

           return behaviour;
        }
        
        private IBehaviour<Monster> ConcussBehaviour()
        {
            var behaviour = FluentBuilder.Create<Monster>()
                .Sequence("concussed")
                .Condition("is concussed", monster => monster.IsConcussed)
                .Condition("roll", monster => GlobalRandom.DefaultRNG.NextInt(100) > 50)
                .Do(
                    "roll",
                    monster =>
                    {
                        GameWorld.MessageLog.AddMessage($"{monster.Name} looks dazed (is concussed)");
                        return BehaviourStatus.Succeeded;
                    }
                )
                .End()
                .Build();

            return behaviour;
        }
        
        private IBehaviour<Monster> MeleeAttackBehaviour()
        {
            var behaviour = FluentBuilder.Create<Monster>()
                .Sequence("melee attack")
                .Condition("has melee attack", monster => MeleeAttack != null)
                .Condition("has target", monster => Target != null)
                .Condition("is hunting", monster => MonsterState == MonsterState.Hunting)
                .Condition("target is adjacent", monster => Position.IsNextTo(Target.Position, AdjacencyRule.EightWay))
                .Do(
                    "attack target",
                    monster =>
                    {
                        var attackCommand = _commandFactory.CreateCommand<MeleeAttackCommand>(GameWorld);
                        attackCommand.Initialise(this, Target, null);
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
                .Condition("has target", monster => Target != null)
                .Condition("hunting", monster => MonsterState == MonsterState.Hunting)
                .Condition("able to attack", monster =>
                {
                    var distanceToTarget = CurrentMap.DistanceMeasurement.Calculate(Position, Target.Position);

                    if (distanceToTarget > 2)
                        return false;
                    
                    if (distanceToTarget <= 1)
                        return true;
                        
                    var directionToTarget = Direction.GetDirection(Position, Target.Position);
                        
                    if (Position + directionToTarget + directionToTarget != Target.Position)
                        return false;

                    // Check for objects blocking the way between target and monster
                    if (CurrentMap.GetTerrainAt<Wall>(Position + directionToTarget) != null)
                        return false;

                    return true;
                })
                .Do("line attack target", monster => ExecuteLineAttack()
                )
                .End()
                .Build();

            return behaviour;
        }

        private BehaviourStatus ExecuteLineAttack()
        {
            var directionToTarget = Direction.GetDirection(Position, Target.Position);
            var targetPoint = Position + directionToTarget + directionToTarget;
            var lineAttackPath = Lines.GetLine(Position, targetPoint)
                .ToList();

            lineAttackPath = lineAttackPath
                .TakeWhile(p => p == Position || (CurrentMap.Contains(targetPoint) && CurrentMap.GetObjectsAt(p).All(o => o.IsGameObjectStrikeThrough())))
                .ToList();
            
            var lineAttackCommand = _commandFactory.CreateCommand<LineAttackCommand>(GameWorld);
            
            lineAttackCommand.Initialise(this, lineAttackPath);
            _nextCommands.Add(lineAttackCommand);

            return BehaviourStatus.Succeeded;
        }

        private IBehaviour<Monster> LightningAttackBehaviour()
        {
            var behaviour = FluentBuilder.Create<Monster>()
                .Sequence("lightning attack")
                .Condition("has lightning attack", monster => LightningAttack != null)
                .Condition("has target", monster => Target != null)
                .Condition("turret or hunting", monster => IsWallTurret || MonsterState == MonsterState.Hunting)
                .Do("lightning attack target", monster => ExecuteLightningAttack()
                )
                .End()
                .Build();

            return behaviour;
        }
        
        private BehaviourStatus ExecuteLightningAttack()
        {
            var lightningAttackCommand = _commandFactory.CreateCommand<LightningAttackCommand>(GameWorld);
            lightningAttackCommand.Initialise(this, Target.Position);

            var targets = lightningAttackCommand.GetTargets();
            if (!targets.Any(t => t is Actor))
                return BehaviourStatus.Failed;

            if (!FriendlyFireAllies)
            {
                var anyAlliesAlongPath = targets.Any(t => t is Monster m && GameWorld.ActorAllegiances.RelationshipTo(this, m) != ActorAllegianceState.Enemy);

                if (anyAlliesAlongPath)
                    return BehaviourStatus.Failed;
            }

            _nextCommands.Add(lightningAttackCommand);

            return BehaviourStatus.Succeeded;
        }

        private IBehaviour<Monster> WanderBehavior()
        {
            var behaviour = FluentBuilder.Create<Monster>()
                .Sequence("wander")
                    .Condition("is wandering", monster => monster.MonsterState == MonsterState.Wandering)
                    .Do(
                        "move to next unexplored square",
                        monster =>
                        {
                            if (Target != null)
                            {
                                var detected = TryDetectTarget();

                                if (detected)
                                {
                                    MonsterState = MonsterState.Hunting;
                                    return BehaviourStatus.Failed;
                                }
                            }

                            // Blocked, can't do anything
                            if (IsBlocked())
                                return BehaviourStatus.Succeeded;

                            var nextDirection = monster.UseGoalMapWander ? WanderUsingGoalMap() : WanderUsingAStar();

                            if (nextDirection == Direction.None)
                            {
                                // move is still blocked for some reason, do nothing
                                return BehaviourStatus.Succeeded;
                            }
                            else
                            {
                                var moveCommand = CreateMoveCommand(nextDirection);
                                _nextCommands.Add(moveCommand);
                            }

                            return BehaviourStatus.Succeeded;
                        })
                .End()
                .Build();

            return behaviour;
        }

        private bool TryDetectTarget()
        {
            if (Target.Position.IsNextTo(Position, AdjacencyRule.EightWay))
            {
                MonsterState = MonsterState.Hunting;

                return true;
            }

            // A Target is always in field of view. If not adjacent then the monster
            // will do a detection check to determine if it transitions to hunting
            // a short range check does not occur for wandering
            var distance = CurrentMap.DistanceMeasurement.Calculate(Position, Target.Position);

            if (distance <= DetectionRange)
            {
                if (GameWorld.Random.NextInt(5, Constants.RngMonsterDetectLongRange) == 0)
                {
                    MonsterState = MonsterState.Hunting;
                    return true;
                }
            }

            return false;
        }

        private bool IsBlocked()
        {
            foreach (var direction in AdjacencyRule.EightWay.DirectionsOfNeighbors())
            {
                if (CurrentMap.GameObjectCanMove(this, Position + direction))
                    return false;
            }

            return true;
        }

        private IBehaviour<Monster> HuntBehaviour()
        {
            var behaviour = FluentBuilder.Create<Monster>()
                .Sequence("hunt")
                    .Condition("hunting", monster => MonsterState == MonsterState.Hunting)
                .Do(
                    "move towards target",
                    monster =>
                    {
                        if (Target == null)
                        {
                            if (TargetOutOfFov != null)
                            {
                                MonsterState = MonsterState.Searching;
                                SearchCooldown = Breed.SearchCooldown;
                            }
                            else
                            {
                                MonsterState = MonsterState.Wandering;
                            }

                            return BehaviourStatus.Failed;
                        }

                        // See if the target is adjacent. If so do nothing. This may happen when
                        // a transition from searching to hunting has occurred.
                        if (Position.Neighbours(CurrentMap, AdjacencyRule.EightWay).Any(p => p == Target.Position))
                        {
                            return BehaviourStatus.Succeeded;
                        }

                        // Target is always in fov. Hunt.
                        var nextDirection = Hunt(Target);

                        // can't move, might be blocked
                        if (nextDirection == Direction.None)
                            return BehaviourStatus.Succeeded;

                        var actorsAtPosition = CurrentMap.GetObjectsAt<Actor>(Position + nextDirection);

                        if (actorsAtPosition.Any())
                        {
                            throw new Exception("Unexpected move - hunt should have blocked a move into any other target");
                        }

                        var moveCommand = CreateMoveCommand(nextDirection);
                        _nextCommands.Add(moveCommand);
                     
                        return BehaviourStatus.Succeeded;
                    }
                )
                .End()
                .Build();

            return behaviour;
        }

        private IBehaviour<Monster> PickTargetBehavior()
        {
            var behaviour = FluentBuilder.Create<Monster>()
                .Sequence("pick target")
                    .Do("pick target", monster =>
                    {
                        // The Target picker will set Target if a target is adjacent or is in
                        // field of view. If a Target was already set but the Target is no longer in field of view
                        // then it will try to pick a new target.  If no other target found it will assign Target
                        // to TargetOutOfFov and clear Target.
                        if (Target != null && Target.IsDead)
                            Target = null;

                        if (TargetOutOfFov != null && TargetOutOfFov.IsDead)
                            TargetOutOfFov = null;

                        // Don't change target if target is already adjacent
                        if (Target != null && Target.Position.IsNextTo(Position, AdjacencyRule.EightWay))
                            return BehaviourStatus.Succeeded;

                        // Check immediate neighbours first
                        var adjacentEnemy = Position.Neighbours(CurrentMap)
                            .Select(p => CurrentMap.GetObjectAt<Actor>(p))
                            .Where(m => m != null)
                            .Where(m => GameWorld.ActorAllegiances.RelationshipTo(this, m) == ActorAllegianceState.Enemy)
                            .FirstOrDefault();

                        if (adjacentEnemy != null)
                        {
                            Target = adjacentEnemy;
                            TargetOutOfFov = null;

                            return BehaviourStatus.Succeeded;
                        }

                        if (Target != null && _fieldOfView.CurrentFOV.Contains(Target.Position))
                        {
                            return BehaviourStatus.Succeeded;
                        }

                        TargetOutOfFov = Target;
                        Target = null;

                        // Check for any actors in field of view
                        var newPotentialTarget = GameWorld.GetCurrentMapActors()
                            .Where(m => _fieldOfView.BooleanResultView[m.Position])
                            .Where(m => GameWorld.ActorAllegiances.RelationshipTo(this, m) == ActorAllegianceState.Enemy)
                            .OrderBy(m => Distance.Chebyshev.Calculate(m.Position, Position))
                            .FirstOrDefault();

                        if (newPotentialTarget != null)
                        {
                            Target = newPotentialTarget;
                            TargetOutOfFov = null;
                        }

                        return BehaviourStatus.Succeeded;
                    })
                .End()
                .Build();

            return behaviour;
        }

        private IBehaviour<Monster> SearchingBehavior()
        {
            // The monster should continue to search for the target
            // until a cooldown timer has expired
            var behaviour = FluentBuilder.Create<Monster>()
                .Sequence("search")
                    .Condition("is searching", monster => MonsterState == MonsterState.Searching)
                .Do(
                    "move towards target",
                    monster =>
                    {
                        // Searching needs to deal with a new Target in field of view
                        // or previous target now out of fov (TargetOutOfFov)
                        if (Target != null)
                        {
                            if (Target.Position.IsNextTo(Position, AdjacencyRule.EightWay))
                            {
                                MonsterState = MonsterState.Hunting;

                                return BehaviourStatus.Failed;
                            }

                            // A Target is always in field of view. If not adjacent then the monster
                            // will do a detection check to determine if it transitions to hunting
                            var distance = CurrentMap.DistanceMeasurement.Calculate(Position, Target.Position);

                            if (distance <= DetectionRange)
                            {
                                if (GameWorld.Random.NextInt(5, Constants.RngMonsterDetectLongRange) == 0)
                                {
                                    MonsterState = MonsterState.Hunting;
                                    return BehaviourStatus.Failed;
                                }
                            }
                            if (distance <= DetectionRange / 2)
                            {
                                // perform a second roll if within half of detection range
                                if (GlobalRandom.DefaultRNG.NextInt(5) == 0)
                                {
                                    MonsterState = MonsterState.Hunting;
                                    return BehaviourStatus.Failed;
                                }
                            }

                            return HuntAndMove(Target);
                        }

                        if (TargetOutOfFov == null)
                        {
                            MonsterState = MonsterState.Wandering;
                            return BehaviourStatus.Failed;
                        }

                        SearchCooldown--;

                        if (SearchCooldown <= 0)
                        {
                            MonsterState = MonsterState.Wandering;

                            // Return failed so that wandering state can run and monster can move
                            return BehaviourStatus.Failed;
                        }

                        return HuntAndMove(TargetOutOfFov);
                    })
                .End()
                .Build();

            return behaviour;
        }

        private BehaviourStatus HuntAndMove(Actor target)
        {
            var nextDirection = Hunt(target);


            if (nextDirection == Direction.None)
                return BehaviourStatus.Failed;

            var actorsAtPosition = CurrentMap.GetObjectsAt<Actor>(Position + nextDirection);

            if (actorsAtPosition.Any())
            {
                // Blocked from moving by another actor, just stand still during this turn
                return BehaviourStatus.Succeeded;
            }

            var moveCommand = CreateMoveCommand(nextDirection);
            _nextCommands.Add(moveCommand);

            return BehaviourStatus.Succeeded;
        }

        public Direction Hunt(Actor target)
        {
            _goalStates.Clear();

            if (target == null)
                throw new Exception("Should not be hunting with no target");

            // Hunt radius only requires the distance between target and monster, plus an extra 3
            // for monsters to be able to surround the target or walk around any blocking enemies.
            // This may limit the monster's ability to walk around blocking terrain if the path around
            // the terrain is not included in this submap, but 99% of cases should be fine, and may
            // sometimes be beneficial as it means monsters can't be tricked into taking a longer
            // path to the player.
            var range = Rectangle.GetIntersection(CurrentMap.RectangleCoveringPoints(target.Position, Position).Expand(3, 3), CurrentMap.Bounds());
            
            for (var x = range.MinExtentX; x <= range.MaxExtentX; x++)
            {
                for (var y = range.MinExtentY; y <= range.MaxExtentY; y++)
                {
                    var gameObjects = CurrentMap.GetObjectsAt(x, y);

                    _goalStates[x, y] = GoalState.Clear;

                    foreach (var gameObject in gameObjects)
                    {
                        if (gameObject == target)
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
            
            Debug.Assert(currentPathLocation != Point.None);

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
                Debug.Fail("Could not find a next point in path and path lookup logic did not return Direction.None. This should never happen.");
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
            Leader = monster;
        }

        public override void ApplyConcussion()
        {
            base.ApplyConcussion();

            if (CanBeConcussed)
                IsConcussed = true;
        }
    }
}