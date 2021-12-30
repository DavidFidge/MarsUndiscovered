using System.Collections.Generic;
using System.Linq;
using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Services;
using MarsUndiscovered.Components.SaveData;

namespace MarsUndiscovered.Components
{
    public class MessageLog : List<MessageLogEntry>, ISaveable
    {
        public void AddMessage(string message)
        {
            Add(new MessageLogEntry(message));
        }

        public void SaveState(ISaveGameStore saveGameStore)
        {
            var messageLogSaveData = this.Select(s => s.Message).ToList();

            saveGameStore.SaveToStore(new Memento<MessageLogSaveData>(new MessageLogSaveData { Messages = messageLogSaveData }));
        }

        public void LoadState(ISaveGameStore saveGameStore)
        {
            foreach (var messageLog in saveGameStore.GetFromStore<MessageLogSaveData>().State.Messages)
            {
                AddMessage(messageLog);
            }
        }
    }
}