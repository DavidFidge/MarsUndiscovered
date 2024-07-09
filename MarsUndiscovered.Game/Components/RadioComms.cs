using FrigidRogue.MonoGame.Core.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Services;
using GoRogue.GameFramework;
using MarsUndiscovered.Game.Commands;
using MarsUndiscovered.Game.Components.SaveData;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Game.Components;

public enum RadioCommsTypes
{
    StartGame1,
    StartGame2,
    PickupShipParts
}

public class RadioComms : List<RadioCommsEntry>, ISaveable
{
    public static string ShipAiSource = "INCOMING MESSAGE FROM MY SHIP AI";
    private int _seenCount;
    private Dictionary<RadioCommsTypes, RadioCommsPrefab> _radioCommsPrefabs = new();

    public RadioComms()
    {
        CreatePrefabs();
    }

    private void CreatePrefabs()
    {
        _radioCommsPrefabs = new Dictionary<RadioCommsTypes, RadioCommsPrefab>();
        
        _radioCommsPrefabs.Add(
            RadioCommsTypes.StartGame1,
            new RadioCommsPrefab(RadioCommsTypes.StartGame1,
                ShipAiSource,
            "Welcome to Mars captain! I apologise for the rough landing. The small matter of the explosion has ripped a hole in the hull and has crippled the primary fuel injection system. Unfortunately we have no spares on board, however the mine nearby likely has a similar controller which I can rig up as a temporary solution to get us flying again. You'll have to put on your spacesuit and walk over there."
                )
            );
        
        _radioCommsPrefabs.Add(
            RadioCommsTypes.StartGame2,
            new RadioCommsPrefab(RadioCommsTypes.StartGame2,
                ShipAiSource,
                "There's no communications signals coming from the mine at all - not even on the encrypted channels. I'm not sure what's going on in there. Be careful, won't you? I don't want to be left forsaken on this cold, barren dust bowl. Or worse, found by scrappers and sold off to the black market. I'll keep in touch on this secure channel."
            )
        );
        
        _radioCommsPrefabs.Add(
            RadioCommsTypes.PickupShipParts,
            new RadioCommsPrefab(RadioCommsTypes.PickupShipParts,
                ShipAiSource,
                "Allow me a minute to scan these parts you've found.........yes, they are adequate for the repairs to the broken fuel injection system. I've identified the hardware and can confirm I am able to interface with these parts. Please bring them back immediately - my repair bots will have the ship prepared for their installation and we will be able to depart within minutes of your arrival."
            )
        );
    }

    private void AddRadioCommsEntry(RadioCommsTypes radioCommsType, IGameObject gameObject)
    {
        var radioCommsPrefab = _radioCommsPrefabs[radioCommsType];
        AddRadioCommsEntry(radioCommsPrefab.RadioCommsType, gameObject, radioCommsPrefab.Message, radioCommsPrefab.Source);
    }

    private void AddRadioCommsEntry(RadioCommsTypes radioCommsType, IGameObject gameObject, MessageLog messageLog)
    {
        var radioCommsPrefab = _radioCommsPrefabs[radioCommsType];
        AddRadioCommsEntry(radioCommsPrefab.RadioCommsType, gameObject, radioCommsPrefab.Message, radioCommsPrefab.Source);
        messageLog.AddMessage($"{radioCommsPrefab.Source}: {radioCommsPrefab.Message}");
    }

    private void AddRadioCommsEntry(RadioCommsTypes radioCommsType, IGameObject gameObject, string message, string source)
    {
        var radioCommsEntry = new RadioCommsEntry(radioCommsType, gameObject, message, source);
        Add(radioCommsEntry);
    }

    public List<RadioCommsEntry> GetNewRadioComms()
    {
        var radioCommsEntries = this.Skip(_seenCount).ToList();
        _seenCount = Count;
        return radioCommsEntries;
    }

    public void CreateGameStartMessages(MessageLog messageLog, Player player)
    {
        AddRadioCommsEntry(RadioCommsTypes.StartGame1, player, messageLog);
        AddRadioCommsEntry(RadioCommsTypes.StartGame2, player, messageLog);
    }

    public void SaveState(ISaveGameService saveGameService, IGameWorld gameWorld)
    {
        var radioCommsItemSaveData = this
            .Select(r =>
                new RadioCommsItemSaveData
                {
                    RadioCommsType = r.RadioCommsType,
                    GameObjectId = r.GameObject.ID
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

        foreach (var radioCommsItem in state.RadioCommsItemSaveData)
            AddRadioCommsEntry(radioCommsItem.RadioCommsType, gameWorld.GameObjects[radioCommsItem.GameObjectId]);

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
                    if (!this.Any(r => r.RadioCommsType == RadioCommsTypes.PickupShipParts))
                    {
                        AddRadioCommsEntry(
                            RadioCommsTypes.PickupShipParts,
                            pickUpItemCommand.Item,
                            messageLog
                        );
                    }
                }
            }
        }
    }
}