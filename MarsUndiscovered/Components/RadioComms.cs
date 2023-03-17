using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Services;
using GoRogue.GameFramework;
using MarsUndiscovered.Commands;
using MarsUndiscovered.Components.SaveData;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Components;

public class RadioComms : List<RadioCommsEntry>, ISaveable
{
    public static string ShipAiSource = "INCOMING MESSAGE FROM YOUR SHIP AI";
    private int _seenCount;
    private Dictionary<int, RadioCommsPrefab> _radioCommsPrefabs = new();

    private const int STARTGAME_1 = 0;
    private const int STARTGAME_2 = 1;
    private const int PICKUP_SHIP_PARTS = 2;

    public RadioComms()
    {
        CreatePrefabs();
    }

    private void CreatePrefabs()
    {
        _radioCommsPrefabs = new Dictionary<int, RadioCommsPrefab>();
        
        _radioCommsPrefabs.Add(
            STARTGAME_1,
            new RadioCommsPrefab(STARTGAME_1,
                ShipAiSource,
            "Welcome to Mars captain! I apologise for the rough landing. The small matter of the explosion has ripped a hole in the hull and has crippled the primary fuel injection system. Unfortunately we have no spares on board, however the mine nearby likely has a similar controller which I can rig up as a temporary solution to get us flying again. You'll have to put on your spacesuit and walk over there."
                )
            );
        
        _radioCommsPrefabs.Add(
            STARTGAME_2,
            new RadioCommsPrefab(STARTGAME_2,
                ShipAiSource,
                "There's no communications signals coming from the mine at all - not even on the encrypted channels. I'm not sure what's going on in there. Be careful, won't you? I don't want to be left forsaken on this cold, barren dust bowl. Or worse, found by scrappers and sold off to the black market. I'll keep in touch on this secure channel."
            )
        );
        
        _radioCommsPrefabs.Add(
            PICKUP_SHIP_PARTS,
            new RadioCommsPrefab(PICKUP_SHIP_PARTS,
                ShipAiSource,
                "Allow me a minute to scan these parts you've found.........yes, they are adequate for the repairs to the broken fuel injection system. I've identified the hardware and can confirm I am able to interface with these parts. Please bring them back immediately - my repair bots will have the ship prepared for their installation and we will be able to depart within minutes of your arrival."
            )
        );
    }

    private void AddRadioCommsEntry(IGameObject gameObject, int radioCommsPrefabId, string message, string source, MessageLog messageLog)
    {
        AddRadioCommsEntryInternal(gameObject, radioCommsPrefabId, message, source);
        messageLog.AddMessage($"{source}: {message}");
    }
    
    private void AddRadioCommsEntry(IGameObject gameObject, RadioCommsPrefab radioCommsPrefab, MessageLog messageLog)
    {
        AddRadioCommsEntryInternal(gameObject, radioCommsPrefab.Id, radioCommsPrefab.Message, radioCommsPrefab.Source);
        messageLog.AddMessage($"{radioCommsPrefab.Source}: {radioCommsPrefab.Message}");
    }
    
    private void AddRadioCommsEntryInternal(IGameObject gameObject, int radioCommsPrefabId, string message, string source)
    {
        var radioCommsEntry = new RadioCommsEntry(radioCommsPrefabId, message, source, gameObject);
        Add(radioCommsEntry);
    }

    public List<RadioCommsEntry> GetNewRadioComms()
    {
        var radioCommsEntries = this.Skip(_seenCount).ToList();
        _seenCount = Count;
        return radioCommsEntries;
    }
        
    public void CreateGameStartMessages(Ship ship, MessageLog messageLog)
    {
        AddRadioCommsEntry(
            ship,
            _radioCommsPrefabs[STARTGAME_1],
            messageLog
        );
            
        AddRadioCommsEntry(
            ship,
            _radioCommsPrefabs[STARTGAME_2],
            messageLog
        );        
    }

    public void SaveState(ISaveGameService saveGameService, IGameWorld gameWorld)
    {
        var radioCommsItemSaveData = this
            .Select(r =>
                new RadioCommsItemSaveData
                {
                    Id = r.Id,
                    GameObjectId = r.GameObject.ID,
                    Message = r.Message,
                    Source = r.Source
                })
            .ToList();

        var radioCommsSaveData = new RadioCommsSaveData
        {
            RadioCommsItemSaveData = radioCommsItemSaveData,
            SeenCount = _seenCount
        };
            
        saveGameService.SaveToStore(new Memento<RadioCommsSaveData>(radioCommsSaveData));
    }

    public void LoadState(ISaveGameService saveGameService, IGameWorld gameWorld)
    {
        var state = saveGameService.GetFromStore<RadioCommsSaveData>().State;

        foreach (var item in state.RadioCommsItemSaveData)
            AddRadioCommsEntryInternal(gameWorld.GameObjects[item.GameObjectId], item.Id, item.Message, item.Source);

        _seenCount = state.SeenCount;
    }

    public void ProcessCommand(BaseGameActionCommand command, MessageLog messageLog)
    {
        if (command is PickUpItemCommand pickUpItemCommand)
        {
            if (pickUpItemCommand.CommandResult.Result == CommandResultEnum.Success)
            {
                if (pickUpItemCommand.Item.ItemType is ShipRepairParts)
                {
                    if (!this.Any(r => r.Id == PICKUP_SHIP_PARTS))
                    {
                        AddRadioCommsEntry(
                            pickUpItemCommand.Item,
                            _radioCommsPrefabs[PICKUP_SHIP_PARTS],
                            messageLog
                        );
                    }
                }
            }
        }
    }
}