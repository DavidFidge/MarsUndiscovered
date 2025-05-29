using BehaviourTree;
using BehaviourTree.FluentBuilder;
using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Services;

using GoRogue.Random;

using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components;

public class Story : BaseComponent, IStory, ISaveable
{
    private IGameWorld _gameWorld;
    private StorySaveData _data;
    private IBehaviour<Story> _behaviourTree;

    public Story()
    {
    }

    public void Initialize(IGameWorld gameWorld)
    {
        _data = new StorySaveData();
        _gameWorld = gameWorld;
        CreateBehaviourTree();
    }

    public void NextTurn()
    {
        _behaviourTree.Tick(this);
    }

    private void CreateBehaviourTree()
    {
        var fluentBuilder = FluentBuilder.Create<Story>();

        _behaviourTree = fluentBuilder
            .Sequence("root")
            .Selector("action selector")
                .Subtree(Level2Behaviour())
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
                    return s._gameWorld.Player.CurrentMap.MarsMap().Level == 2; 
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
                        return GlobalRandom.DefaultRNG.NextInt(2) == 0;
                    })
                    .Do("Add missiles",
                        s =>
                        {
                            SpawnMissiles();

                            return BehaviourStatus.Succeeded;
                        }
                    )
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
    }
}