using System.Threading;
using FrigidRogue.MonoGame.Core.Components.Mediator;
using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

namespace MarsUndiscovered.UserInterface.Views
{
    public class InventoryReplayView : BaseInventoryView<InventoryReplayViewModel, InventoryReplayData>,
        IRequestHandler<LeftClickInventoryReplayViewRequest>
    {
        public InventoryReplayView(
            InventoryReplayViewModel inventoryReplayViewModel
        ) : base(inventoryReplayViewModel)
        {
        }
        
        public void Handle(LeftClickInventoryReplayViewRequest request)
        {
            HideIfMouseOver();
        }
    }
}