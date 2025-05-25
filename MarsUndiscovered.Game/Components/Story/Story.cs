using BehaviourTree;
using BehaviourTree.FluentBuilder;
using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Services;
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
            .Condition("is active", s => s._data.IsLevel2StoryActive)
            .Condition("on level 2", s => s._gameWorld.Player.CurrentMap.MarsMap().Level == 2)
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
                .Condition("on level 2", s => s._gameWorld.Player.CurrentMap.MarsMap().Level == 2)
                .Sequence("start level 2")
                    .Condition("has visited mining facility", s => !s._data.HasVisitedMiningFacilityOutside)
                    .Do(
                        "comms message and flag",
                        s =>
                        {
                            s._data.HasVisitedMiningFacilityOutside = true;
                            _gameWorld.RadioComms.AddRadioCommsEntry(RadioCommsTypes.Level2StoryStart, _gameWorld.Player);
                            return BehaviourStatus.Succeeded;
                        }
                    )
                    .Do("Add missiles",
                        s =>
                        {
                            var environmentalEffectParams = new SpawnEnvironmentalEffectParams()
                                .WithEnvironmentalEffectType(EnvironmentalEffectType.MissileTargetType)
                                .OnMap(_gameWorld.CurrentMap.Id);

                            _gameWorld.SpawnEnvironmentalEffect(environmentalEffectParams);

                            environmentalEffectParams.Result.Duration = 2;

                            return BehaviourStatus.Succeeded;
                        }
                    )
                
            .End()
            .Build();

        return behaviour;
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