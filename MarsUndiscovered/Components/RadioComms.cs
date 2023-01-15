using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Services;
using GoRogue.GameFramework;
using MarsUndiscovered.Components.SaveData;

namespace MarsUndiscovered.Components
{
    public class RadioComms : List<RadioCommsEntry>, ISaveable
    {
        public void AddRadioCommsEntry(string message, IGameObject gameObject)
        {
            Add(new RadioCommsEntry(message, gameObject));
        }

        public void AddRadioCommsEntry(IList<string> messages, IList<IGameObject> gameObjects)
        {
            for (var index = 0; index < messages.Count; index++)
            {
                Add(new RadioCommsEntry(messages[index], gameObjects[index]));
            }
        }

        public void SaveState(ISaveGameService saveGameService)
        {
            var messages = new List<string>(Count);
            var gameObjectIds = new List<uint>(Count);

            foreach (var message in this)
            {
                messages.Add(message.Message);
                gameObjectIds.Add(message.GameObject.ID);
            }
            
            saveGameService.SaveToStore(new Memento<RadioCommsSaveData>(new RadioCommsSaveData { Messages = messages, GameObjectIds = gameObjectIds}));
        }

        public void LoadState(ISaveGameService saveGameService)
        {
            var state = saveGameService.GetFromStore<RadioCommsSaveData>().State;
            
            for (var i = 0; i < state.Messages.Count; i++)
            {
                this.;
                AddRadioCommsEntry(state.Messages[i], state.GameObjectIds[i]);
            }
        }
    }
}