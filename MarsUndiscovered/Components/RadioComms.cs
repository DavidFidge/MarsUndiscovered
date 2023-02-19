using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Services;
using GoRogue.GameFramework;
using MarsUndiscovered.Components.SaveData;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Components
{
    public class RadioComms : List<RadioCommsEntry>, ISaveable
    {
        public static string ShipAiSource = "INCOMING MESSAGE FROM YOUR SHIP AI";
        
        public void AddRadioCommsEntry(IGameObject gameObject, string message, string source)
        {
            Add(new RadioCommsEntry(message, source, gameObject));
        }

        public void SaveState(ISaveGameService saveGameService, IGameWorld gameWorld)
        {
            var saveData = this
                .Select(r =>
                    new RadioCommsSaveData
                    {
                        GameObjectId = r.GameObject.ID,
                        Message = r.Message,
                        Source = r.Source
                    })
                .ToList();
            
            saveGameService.SaveToStore(new Memento<List<RadioCommsSaveData>>(saveData));
        }

        public void LoadState(ISaveGameService saveGameService, IGameWorld gameWorld)
        {
            var state = saveGameService.GetFromStore<List<RadioCommsSaveData>>().State;

            foreach (var item in state)
            {
                AddRadioCommsEntry(gameWorld.GameObjects[item.GameObjectId], item.Message, item.Source);
            }
        }
    }
}