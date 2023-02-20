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
        private int _seenCount;
        
        public void AddRadioCommsEntry(IGameObject gameObject, string message, string source, MessageLog messageLog)
        {
            AddRadioCommsEntryInternal(gameObject, message, source);
            messageLog.AddMessage($"{source}: {message}");
        }

        private void AddRadioCommsEntryInternal(IGameObject gameObject, string message, string source)
        {
            var radioCommsEntry = new RadioCommsEntry(message, source, gameObject);
            Add(radioCommsEntry);
        }

        public List<RadioCommsEntry> GetNewRadioComms()
        {
            var radioCommsEntries = this.Skip(_seenCount).ToList();
            _seenCount = Count;
            return radioCommsEntries;
        }

        public void SaveState(ISaveGameService saveGameService, IGameWorld gameWorld)
        {
            var radioCommsItemSaveData = this
                .Select(r =>
                    new RadioCommsItemSaveData
                    {
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
                AddRadioCommsEntryInternal(gameWorld.GameObjects[item.GameObjectId], item.Message, item.Source);

            _seenCount = state.SeenCount;
        }
    }
}