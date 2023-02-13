using System.Threading;
using System.Threading.Tasks;
using MarsUndiscovered.UserInterface.Data;

using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Services;
using FrigidRogue.MonoGame.Core.UserInterface;

using MediatR;

namespace MarsUndiscovered.UserInterface.ViewModels
{
    public class GameOptionsViewModel : BaseViewModel<GameOptionsData>
    {
        private readonly IGameOptionsStore _gameOptionsStore;

        public GameOptionsViewModel(IGameOptionsStore gameOptionsStore)
        {
            _gameOptionsStore = gameOptionsStore;
        }

        public override void Initialize()
        {
            Data = _gameOptionsStore.GetFromStore<GameOptionsData>()?.State ?? new GameOptionsData();
        }

        public override Task<Unit> Handle(InterfaceRequest<GameOptionsData> request, CancellationToken cancellationToken)
        {
            base.Handle(request, cancellationToken);

            _gameOptionsStore.SaveToStore(new Memento<GameOptionsData>(Data));

            return Unit.Task;
        }
    }
}