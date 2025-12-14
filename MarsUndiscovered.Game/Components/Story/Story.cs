using BehaviourTree;
using BehaviourTree.FluentBuilder;
using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Services;

using GoRogue.Random;

using MarsUndiscovered.Interfaces;

using SadRogue.Primitives;

namespace MarsUndiscovered.Game.Components;

public class Story : BaseComponent, IStory, ISaveable
{
    private IGameWorld _gameWorld;
    private StorySaveData _data;
    private IBehaviour<Story> _behaviourTree;

    private Monster _level2MinerLeader { get; set; }

    public Story()
    {
    }

    public void Initialize(IGameWorld gameWorld)
    {
        _data = new StorySaveData();
        _gameWorld = gameWorld;
        CreateBehaviourTree();
    }

    public void NewGame(IGameWorld gameWorld)
    {
        _level2MinerLeader = gameWorld.Monsters.Values
            .Where(m => m.CurrentMap.MarsMap().Level == 2)
            .Where(m => m.Breed == Breed.GetBreed("Ricky"))
            .First();

        _data.Level2MinerLeaderId = _level2MinerLeader.ID;
    }

    public void NextTurn()
    {
        _data.LastLevel = _data.CurrentLevel;
        _data.CurrentLevel = _gameWorld.CurrentMap.Level;

        _behaviourTree.Tick(this);
    }

    private void CreateBehaviourTree()
    {
        var fluentBuilder = FluentBuilder.Create<Story>();

        _behaviourTree = fluentBuilder
            .Sequence("root")
            .Selector("action selector")
                .Subtree(Level2Behaviour())
                .Subtree(Level3Behaviour())
                .End()
            .End()
            .Build();
    }
 
    private IBehaviour<Story> Level2Behaviour()
    {
        var behaviour = FluentBuilder.Create<Story>()
            .Sequence("level 2")
                .Condition("is active", s => s._data.IsLevel2StoryActive)
                .Condition("on level 2", s =>
                {
                    return s._gameWorld.CurrentMap.Level == 2;
                })
                // If not a Do() then further leaves need to be put into a subtree
                .Subtree(Level2StorySelector())
            .End()
            .Build();

        return behaviour;
    }

    private IBehaviour<Story> Level2StorySelector()
    {
        var behaviour = FluentBuilder.Create<Story>()
            .Selector("level 2 story selector")
                .Sequence("start level 2")
                    .Condition("has visited mining facility", s =>
                    {
                        return !s._data.HasVisitedMiningFacilityOutside;
                    })
                    .Do(
                        "comms message and flag",
                        s =>
                        {
                            s._data.HasVisitedMiningFacilityOutside = true;
                            _gameWorld.RadioComms.AddRadioCommsEntry(RadioCommsTypes.Level2StoryStart, _gameWorld.Player);
                            return BehaviourStatus.Succeeded;
                        }
                    )
                    .End()
                .Sequence("Spawn Missiles")
                    .Condition("Randomise spawn", s =>
                    {
                        return GlobalRandom.DefaultRNG.NextInt(6) == 0;
                    })
                    .Do("Add missiles",
                        s =>
                        {
                            SpawnMissiles();

                            return BehaviourStatus.Succeeded;
                        }
                    )
                    .End()
                .Sequence("Meet Miners")
                    .Condition("has not met leader", s =>
                    {
                        return !_data.HasMetMinerLeader;
                    })
                    .Do("try meet leader",
                        s =>
                        {
                            if (_gameWorld.CurrentMap.PlayerFOV.CurrentFOV.Contains(_level2MinerLeader.Position))
                            {
                                _data.HasMetMinerLeader = true;

                                _gameWorld.RadioComms.AddRadioCommsEntry(RadioCommsTypes.MetMiners1, _level2MinerLeader);
                                _gameWorld.RadioComms.AddRadioCommsEntry(RadioCommsTypes.MetMiners2, _level2MinerLeader);

                                _level2MinerLeader.SetLeader(_gameWorld.Player);
                            }

                            return BehaviourStatus.Succeeded;
                        }
                    )
                    .End()
            .End()
            .Build();

        return behaviour;
    }

    private IBehaviour<Story> Level3Behaviour()
    {
        var behaviour = FluentBuilder.Create<Story>()
            .Sequence("level 3")
                .Condition("is active", s => s._data.IsLevel3StoryActive)
                .Condition("on level 3", s =>
                {
                    return s._gameWorld.Player.CurrentMap.MarsMap().Level == 3;
                })
                // If not a Do() then further leaves need to be put into a subtree
                .Subtree(Level3StorySelector())
            .End()
            .Build();

        return behaviour;
    }

    private IBehaviour<Story> Level3StorySelector()
    {
        var behaviour = FluentBuilder.Create<Story>()
            .Selector("level 3 story selector")
                .Sequence("start level 3")
                    .Condition("has not yet guided miner leader down", s =>
                    {
                        return !s._data.HasGuidedMinerLeaderDown;
                    })
                    .Condition("miner leader not dead", s =>
                    {
                        return !_level2MinerLeader.IsDead;
                    })
                    .Condition("has met miner leader", s =>
                    {
                        return _data.HasMetMinerLeader;
                    })
                    .Do(
                        "comms message and flag",
                        s =>
                        {
                            // See if the miner leader is within 5 squares of the level 2 exit and if so
                            // switch him and any miners down to this level
                            var level2Map = _gameWorld.Maps.First(m => m.Level == 2);
                            var exit = _gameWorld.MapExits.ForMap(level2Map).First(m => m.Destination.CurrentMap == _gameWorld.CurrentMap);

                            var leader = _level2MinerLeader;

                            if (Distance.Manhattan.Calculate(leader.Position, exit.Position) <= 5)
                            {
                                var freeFloor = _gameWorld.CurrentMap.FindClosestFreeFloor(_gameWorld.Player.Position);

                                if (freeFloor != Point.None)
                                {
                                    _data.HasGuidedMinerLeaderDown = true;
                                    _data.IsLevel2StoryActive = false;

                                    leader.ChangeMaps(_gameWorld.CurrentMap, freeFloor);

                                    // Move miners following leader
                                    var minerFollowers = _gameWorld.Monsters.Values
                                        .Where(m => m.Leader == leader)
                                        .Where(m => !m.IsDead)
                                        .Where(m => m.CurrentMap != null)
                                        .ToList();

                                    foreach (var minerFollower in minerFollowers)
                                    {
                                        freeFloor = _gameWorld.CurrentMap.FindClosestFreeFloor(_gameWorld.Player.Position);

                                        if (freeFloor != Point.None)
                                        {
                                            minerFollower.ChangeMaps(_gameWorld.CurrentMap, freeFloor);
                                        }
                                    }

                                    leader.SetLeader(null);

                                    leader.TravelTarget = _gameWorld.Waypoints.Values.Single(m => m.Name == Constants.WaypointCanteen).Position;

                                    _gameWorld.RadioComms.AddRadioCommsEntry(RadioCommsTypes.GuidedMinerLeaderDown, s._gameWorld.Player);
                                }
                            }
                            return BehaviourStatus.Succeeded;
                        }
                    )
                    .End()
                .Sequence("miners escorted to canteen")
                    .Condition("has not guided miner leader to canteen", s =>
                    {
                        return !s._data.HasGuidedMinerLeaderToCanteen;
                    })
                    .Condition("has guided miner leader down", s =>
                    {
                        return s._data.HasGuidedMinerLeaderDown;
                    })
                    .Condition("miner leader not dead", s =>
                    {
                        return !_level2MinerLeader.IsDead;
                    })
                    .Condition("miner leader reached target", s =>
                    {
                        return _level2MinerLeader.TravelTarget == Point.None;
                    })
                    .Condition("player nearby", s =>
                    {
                        return ChebyshevDistance.Chebyshev.Calculate(_level2MinerLeader.Position, _gameWorld.Player.Position) <= 2;
                    })
                    .Do(
                        "comms message for guided miner leader to canteen",
                        s =>
                        {
                            s._data.HasGuidedMinerLeaderToCanteen = true;

                            _gameWorld.RadioComms.AddRadioCommsEntry(RadioCommsTypes.GuidedMinerLeaderToCanteen, s._gameWorld.Player);

                            return BehaviourStatus.Succeeded;
                        })
                    .End()
            .End()
            .Build();

        return behaviour;
    }

    private void SpawnMissiles()
    {
        var randomMapPosition = _gameWorld.CurrentMap.RandomPosition();

        var points = _gameWorld.CurrentMap.CircleCoveringPoints(randomMapPosition, 5);

        foreach (var point in points)
        {
            var monster = _gameWorld.CurrentMap.GetObjectAt<Monster>(point);

            if (monster != null)
            {
                if (_gameWorld.ActorAllegiances.RelationshipTo(AllegianceCategory.Player, monster.AllegianceCategory) != ActorAllegianceState.Enemy)
                {
                    // at this stage don't put a missile on top of player allies
                    return;
                }
            }
        }

        foreach (var point in points)
        {
            if (_gameWorld.CurrentMap.GetObjectAt<EnvironmentalEffect>(point) != null)
            {
                continue; // Skip if there's already an environmental effect here
            }

            var environmentalEffectParams = new SpawnEnvironmentalEffectParams()
                .WithEnvironmentalEffectType(EnvironmentalEffectType.MissileTargetType)
                .AtPosition(point)
                .OnMap(_gameWorld.CurrentMap.Id);

            _gameWorld.SpawnEnvironmentalEffect(environmentalEffectParams);

            environmentalEffectParams.Result.Duration = 3;
        }
    }

    public void SaveState(ISaveGameService saveGameService, IGameWorld gameWorld)
    {
        var storySaveData = new Memento<StorySaveData>(_data);
        
        saveGameService.SaveToStore(storySaveData);
    }

    public void LoadState(ISaveGameService saveGameService, IGameWorld gameWorld)
    {
        _gameWorld = gameWorld;
        _data = saveGameService.GetFromStore<StorySaveData>().State;

        _level2MinerLeader = _gameWorld.Monsters[_data.Level2MinerLeaderId];
    }
}