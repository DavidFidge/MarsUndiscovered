using MarsUndiscovered.Components.Dto;
using MarsUndiscovered.UserInterface.Data;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public abstract class BaseGameViewModel<T> : BaseGameCoreViewModel<T>
        where T : BaseGameData, new()
    {
        private List<RadioCommsItem> _radioCommsItems = new();
        public PlayerStatus PlayerStatus { get; set; }
        public IList<MonsterStatus> MonsterStatusInView { get; set; }

        public MessagesStatus MessageStatus { get; set; }
        
        protected void SetUpViewModels()
        {
            MessageStatus = new MessagesStatus();
            SetUpGameCoreViewModels();
        }

        protected override void RefreshView()
        {
            MonsterStatusInView = GameWorldEndpoint.GetStatusOfMonstersInView();
            PlayerStatus = GameWorldEndpoint.GetPlayerStatus();
            _radioCommsItems.AddRange(GameWorldEndpoint.GetNewRadioCommsItems());
            MessageStatus.AddMessages(GameWorldEndpoint.GetMessagesSince(MessageStatus.SeenMessageCount));
            MapViewModel.UpdateDebugTiles();
        }

        public IList<RadioCommsItem> GetNewRadioCommsItems()
        {
            var radioCommsItems = _radioCommsItems.ToList();
            _radioCommsItems.Clear();
            return radioCommsItems;
        }
    }
}