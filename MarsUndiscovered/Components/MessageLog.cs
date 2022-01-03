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

        public void AddMessages(IList<string> messages)
        {
            foreach (var message in messages)
            {
                Add(new MessageLogEntry(message));
            }
        }

        public void SaveState(ISaveGameService saveGameService)
        {
            var messageLogSaveData = this.Select(s => s.Message).ToList();

            saveGameService.SaveToStore(new Memento<MessageLogSaveData>(new MessageLogSaveData { Messages = messageLogSaveData }));
        }

        public void LoadState(ISaveGameService saveGameService)
        {
            foreach (var messageLog in saveGameService.GetFromStore<MessageLogSaveData>().State.Messages)
            {
                AddMessage(messageLog);
            }
        }
    }
}