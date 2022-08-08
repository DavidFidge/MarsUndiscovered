using System.Collections.Generic;
using MarsUndiscovered.Components;
using MarsUndiscovered.UserInterface.Data;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public abstract class BaseGameViewModel<T> : BaseGameCoreViewModel<T>
        where T : BaseGameData, new()
    {
        public PlayerStatus PlayerStatus { get; set; }
        public IList<MonsterStatus> MonsterStatusInView { get; set; }

        public IList<string> Messages { get; set; }

        protected int MessageLogCount;

        protected void SetUpViewModels()
        {
            SetUpGameCoreViewModels();
            MessageLogCount = 0;
        }

        protected override void RefreshView()
        {
            MonsterStatusInView = GameWorldEndpoint.GetStatusOfMonstersInView();
            PlayerStatus = GameWorldEndpoint.GetPlayerStatus();

            Messages = GameWorldEndpoint.GetMessagesSince(MessageLogCount);
            MessageLogCount += Messages.Count;
        }
    }
}