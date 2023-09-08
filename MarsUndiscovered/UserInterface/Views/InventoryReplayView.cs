using System.Threading;
using System.Threading.Tasks;
using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;
using MediatR;

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
        
        public Task<Unit> Handle(LeftClickInventoryReplayViewRequest request, CancellationToken cancellationToken)
        {
            HideIfMouseOver();

            return Unit.Task;
        }
    }
}