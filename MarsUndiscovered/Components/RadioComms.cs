using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Services;
using GoRogue.GameFramework;
using MarsUndiscovered.Components.SaveData;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.Components
{
    public class RadioComms : List<RadioCommsEntry>, ISaveable
    {
        public void AddRadioCommsEntry(IGameObject gameObject, string message)
        {
            Add(new RadioCommsEntry(message, gameObject));
        }

        public void SaveState(ISaveGameService saveGameService, IGameWorld gameWorld)
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

        public void LoadState(ISaveGameService saveGameService, IGameWorld gameWorld)
        {
            var state = saveGameService.GetFromStore<RadioCommsSaveData>().State;
            
            for (var i = 0; i < state.Messages.Count; i++)
            {
                AddRadioCommsEntry(gameWorld.GameObjects[(uint)i], state.Messages[i]);
            }
        }
    }
}