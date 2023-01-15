using MarsUndiscovered.Components;
using MarsUndiscovered.Components.Dto;
using MarsUndiscovered.UserInterface.Data;
using Microsoft.Xna.Framework.Graphics;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public abstract class BaseGameViewModel<T> : BaseGameCoreViewModel<T>
        where T : BaseGameData, new()
    {
        public PlayerStatus PlayerStatus { get; set; }
        public IList<MonsterStatus> MonsterStatusInView { get; set; }

        public MessagesStatus MessageStatus { get; set; }
        public RadioCommsStatus RadioCommsStatus { get; set; }
        
        protected void SetUpViewModels()
        {
            MessageStatus = new MessagesStatus();
            SetUpGameCoreViewModels();
        }

        protected override void RefreshView()
        {
            MonsterStatusInView = GameWorldEndpoint.GetStatusOfMonstersInView();
            PlayerStatus = GameWorldEndpoint.GetPlayerStatus();
            RadioCommsStatus.AddRadioCommsItems(GameWorldEndpoint.GetRadioCommsItemsSince(RadioCommsStatus.SeenItemsCount));
            MessageStatus.AddMessages(GameWorldEndpoint.GetMessagesSince(MessageStatus.SeenMessageCount));
        }
    }
}