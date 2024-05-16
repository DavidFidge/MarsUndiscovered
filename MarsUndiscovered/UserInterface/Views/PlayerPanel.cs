using MarsUndiscovered.Game.Components.Dto;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.UserInterface.Views
{
    public class PlayerPanel : ActorPanel<PlayerStatus>
    {
        public PlayerPanel(IAssets assets) : base(assets)
        {
        }
    }
}