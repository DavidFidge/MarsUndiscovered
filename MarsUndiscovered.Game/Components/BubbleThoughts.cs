using BehaviourTree;
using BehaviourTree.FluentBuilder;
using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Services;
using GoRogue.GameFramework;
using MarsUndiscovered.Game.Components.SaveData;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components;

public enum BubbleThoughtTypes
{
    DroidsOutOfControl
}

public class BubbleThoughts : ISaveable
{
    private Dictionary<BubbleThoughtTypes, BubbleThoughtEntry> _bubbleThoughtEntries = new();
    private List<BubbleThoughtEntry> _newBubbleThoughts = new();
    private IGameWorld _gameWorld;
    private IBehaviour<BubbleThoughts> _behaviourTree;

    public BubbleThoughts(IGameWorld gameWorld)
    {
        _gameWorld = gameWorld;
        CreateEntries();
        CreateBehaviourTree();
    }

    public void NextTurn()
    {
        _behaviourTree.Tick(this);
    }

    public IList<BubbleThoughtEntry> GetNewBubbleThoughts()
    {
        var bubbleThoughts = _newBubbleThoughts.ToList();
        _newBubbleThoughts.Clear();
        return bubbleThoughts;
    }

    private void CreateBehaviourTree()
    {
        _behaviourTree = FluentBuilder.Create<BubbleThoughts>()
            .Sequence("root")
                .Subtree(Level1Behavior())
            .End()
            .Build();
    }

    private IBehaviour<BubbleThoughts> Level1Behavior()
    {
        return FluentBuilder.Create<BubbleThoughts>()
            .Sequence("level 1")
                .Condition("on level 1 or level 2", s => s._gameWorld.CurrentMap.Level == 1 || s._gameWorld.CurrentMap.Level == 2)
                .Condition("not already seen", s => !s._bubbleThoughtEntries[BubbleThoughtTypes.DroidsOutOfControl].HasBeenSeen)
                .Condition("player sees repair droid", s =>
                    s._gameWorld.GetCurrentMapMonsters()
                        .Where(m => s._gameWorld.CurrentMap.PlayerFOV.CurrentFOV.Contains(m.Position))
                        .Any(m => m.Breed.Name == Breed.GetBreed("Repair Droid").Name))
                .Do("mark bubble thought as seen", s =>
                {
                    var bubbleThought = s._bubbleThoughtEntries[BubbleThoughtTypes.DroidsOutOfControl];
                    bubbleThought.HasBeenSeen = true;
                    s._newBubbleThoughts.Add(bubbleThought);
                    return BehaviourStatus.Succeeded;
                })
            .End()
            .Build();
    }

    private void CreateEntries()
    {
        _bubbleThoughtEntries = new Dictionary<BubbleThoughtTypes, BubbleThoughtEntry>();

        _bubbleThoughtEntries.Add(
            BubbleThoughtTypes.DroidsOutOfControl,
            new BubbleThoughtEntry(BubbleThoughtTypes.DroidsOutOfControl,
                "They said it was not possible, but these cleaning droids are attacking humans! This would require an entire rewrite of the droid's AI software as well as bypassing multiple levels of hardware protection layers. This is just a nondescript mine out in a remote part of Mars. Why would anyone out here care for hacking cleaning droids to kill humans?"
            )
        );
    }

    public void SaveState(ISaveGameService saveGameService, IGameWorld gameWorld)
    {
        var bubbleThoughtEntrySaveData = _bubbleThoughtEntries.Values
            .Select(b =>
                new BubbleThoughtEntrySaveData
                {
                    BubbleThoughtType = b.BubbleThoughtType,
                    HasBeenSeen = b.HasBeenSeen
                })
            .ToList();

        var bubbleThoughtSaveData = new BubbleThoughtSaveData
        {
            BubbleThoughtEntrySaveData = bubbleThoughtEntrySaveData
        };

        saveGameService.SaveToStore(new Memento<BubbleThoughtSaveData>(bubbleThoughtSaveData));
    }

    public void LoadState(ISaveGameService saveGameService, IGameWorld gameWorld)
    {
        _gameWorld = gameWorld;

        var state = saveGameService.GetFromStore<BubbleThoughtSaveData>().State;

        foreach (var bubbleThoughtEntry in state.BubbleThoughtEntrySaveData)
        {
            _bubbleThoughtEntries[bubbleThoughtEntry.BubbleThoughtType].HasBeenSeen = bubbleThoughtEntry.HasBeenSeen;
        }
    }
}