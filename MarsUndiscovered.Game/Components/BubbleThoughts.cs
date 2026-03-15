using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Services;
using GoRogue.GameFramework;
using MarsUndiscovered.Game.Commands;
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
    private IGameWorld _gameWorld;
    
    public BubbleThoughts(IGameWorld gameWorld)
    {
        _gameWorld = gameWorld;
        CreateEntries();
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