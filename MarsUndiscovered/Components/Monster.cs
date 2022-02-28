using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BehaviourTree;
using BehaviourTree.FluentBuilder;

using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Services;

using GoRogue.FOV;
using GoRogue.GameFramework;
using GoRogue.Pathing;

using MarsUndiscovered.Commands;
using MarsUndiscovered.Components.Factories;
using MarsUndiscovered.Components.SaveData;
using MarsUndiscovered.Extensions;
using MarsUndiscovered.Interfaces;

using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MarsUndiscovered.Components
{
    public class Monster : Actor, IMementoState<MonsterSaveData>
    {
        public Breed Breed { get; set; }
        public override string Name => Breed.Name;
        public override string Description => Breed.Description;
        public override Attack BasicAttack => Breed.BasicAttack;
        public override LightningAttack LightningAttack => Breed.LightningAttack;
        public override bool IsWallTurret => Breed.IsWallTurret;

        public bool FriendlyFireAllies => Breed.FriendlyFireAllies;

        private IFOV _fieldOfView;
        private ArrayView<SeenTile> _seenTiles;
        private ArrayView<GoalState> _goalStates;
        private GoalMap _chebyshevGoalState;
        private GoalMap _manhattanGoalState;
        private WeightedGoalMap _goalMap;
        private IBehaviour<Monster> _behaviourTree;
        private IList<BaseGameActionCommand> _nextCommands;
        private ICommandFactory _commandFactory;

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

            if (BasicAttack != null)
            {
                maxDamage = BasicAttack.DamageRange.Max;
                percentMaxDamage = BasicAttack.DamageRange.Max * 100 / player.MaxHealth;
                percentMinDamage = BasicAttack.DamageRange.Min * 100 / player.MaxHealth;
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
            _goalStates = new ArrayView<GoalState>(MarsMap.MapWidth, MarsMap.MapHeight);
            _chebyshevGoalState = new GoalMap(_goalStates, Distance.Chebyshev);
            _manhattanGoalState = new GoalMap(_goalStates, Distance.Manhattan);
            _goalMap = new WeightedGoalMap(new[] { _chebyshevGoalState, _manhattanGoalState });
            _nextCommands = new List<BaseGameActionCommand>();
        }

        public Monster WithBreed(Breed breed)
        {
            Breed = breed;
            MaxHealth = (int)(BaseHealth * Breed.HealthModifier);
            Health = MaxHealth;
            CreateBehaviourTree();

            return this;
        }

        public Monster AddToMap(MarsMap marsMap)
        {
            // Normally actors are not walkable as they can't be on the same square, but if an actor is on a wall it has to be walkable so that
            // it can be on the same square as a (non-walkable) wall.
            if (IsWallTurret)
                IsWalkable = true; 

            MarsGameObjectFluentExtensions.AddToMap(this, marsMap);

            _fieldOfView = new RecursiveShadowcastingFOV(marsMap.TransparencyView);
            _seenTiles = SeenTile.CreateArrayViewFromMap(marsMap);

            return this;
        }

        public void SetLoadState(IMemento<MonsterSaveData> memento)
        {
            PopulateLoadState(memento.State);
            Breed = Breed.Breeds[memento.State.BreedName];

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
        }

        public IMemento<MonsterSaveData> GetSaveState()
        {
            var memento = new Memento<MonsterSaveData>(new MonsterSaveData());

            base.PopulateSaveState(memento.State);

            memento.State.BreedName = Breed.Name;
            memento.State.SeenTiles = _seenTiles.ToArray()
                .Select(s => s.GetSaveState())
                .ToList();

            return memento;
        }

        public override void AfterMapLoaded()
        {
            base.AfterMapLoaded();

            _fieldOfView = new RecursiveShadowcastingFOV(CurrentMap.TransparencyView);
        }

        public IEnumerable<BaseGameActionCommand> NextTurn(ICommandFactory commandFactory)
        {
            _commandFactory = commandFactory;
            _fieldOfView.Calculate(Position);
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
                .Condition("on same map as player", monster => CurrentMap.Equals(GameWorld.Player.CurrentMap))
                .Selector("action selector")
                    .Subtree(BasicAttackBehaviour())
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
                        .Subtree(WanderBehavior())
                    .End()
                .End()
                .Build();

            return behaviour;
        }

        private IBehaviour<Monster> BasicAttackBehaviour()
        {
            var behaviour = FluentBuilder.Create<Monster>()
                .Sequence("melee attack")
                .Condition("has basic attack", monster => BasicAttack != null)
                .Condition("player is adjacent", monster => Position.IsNextTo(GameWorld.Player.Position, AdjacencyRule.EightWay))
                .Do(
                    "attack player",
                    monster =>
                    {
                        var attackCommand = _commandFactory.CreateAttackCommand(GameWorld);
                        attackCommand.Initialise(this, GameWorld.Player);
                        _nextCommands.Add(attackCommand);

                        return BehaviourStatus.Succeeded;
                    }
                )
                .End()
                .Build();

            return behaviour;
        }

        private IBehaviour<Monster> LightningAttackBehaviour()
        {
            var behaviour = FluentBuilder.Create<Monster>()
                .Sequence("lightning attack")
                .Condition("has lightning attack", monster => LightningAttack != null)
                .Condition("player is in field of view", monster => _fieldOfView.CurrentFOV.Contains(GameWorld.Player.Position))
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

            if (!FriendlyFireAllies)
            {
                var anyAlliesAlongPath = lightningAttackCommand.Targets.Any(t => t is Monster);

                if (anyAlliesAlongPath)
                    return BehaviourStatus.Failed;
            }

            _nextCommands.Add(lightningAttackCommand);

            return BehaviourStatus.Succeeded;
        }

        private IBehaviour<Monster> WanderBehavior()
        {
            var behaviour = FluentBuilder.Create<Monster>()
                .Selector("wander")
                    .Do(
                        "move to next unexplored square",
                        monster =>
                        {
                            var nextDirection = Wander();

                            if (nextDirection == Direction.None)
                                return BehaviourStatus.Failed;

                            var moveCommand = CreateMoveCommand(_commandFactory, nextDirection);
                            _nextCommands.Add(moveCommand);

                            return BehaviourStatus.Succeeded;
                        }
                    )
                    .Sequence("rebuild field of view sequence")
                    .Condition(
                        "is blocked",
                        monster =>
                        {
                            foreach (var direction in AdjacencyRule.EightWay.DirectionsOfNeighbors())
                            {
                                if (CurrentMap.GameObjectCanMove(this, Position + direction))
                                    return false;
                            }

                            return true;
                        }
                    )
                    .Do(
                        "Rebuild fieldOfView",
                        monster =>
                        {
                            ResetFieldOfViewAndSeenTiles();
                            return BehaviourStatus.Succeeded;
                        }
                    )
                    .End()
                .End()
                .Build();

            return behaviour;
        }

        private IBehaviour<Monster> HuntBehaviour()
        {
            var behaviour = FluentBuilder.Create<Monster>()
                .Sequence("hunt")
                    .Condition("player in field of view", monster => _fieldOfView.CurrentFOV.Contains(GameWorld.Player.Position))
                    .Do(
                        "move towards player",
                        monster =>
                        {
                            var nextDirection = Hunt();

                            if (nextDirection == Direction.None)
                                return BehaviourStatus.Failed;

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

            for (var x = 0; x < CurrentMap.Width; x++)
            {
                for (var y = 0; y < CurrentMap.Height; y++)
                {
                    var gameObjects = CurrentMap
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

            for (var x = 0; x < CurrentMap.Width; x++)
            {
                for (var y = 0; y < CurrentMap.Height; y++)
                {
                    if (!_seenTiles[x, y].HasBeenSeen)
                    {
                        _goalStates[x, y] = GoalState.Goal;
                        continue;
                    }

                    var gameObjects = CurrentMap
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