using System.Threading;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Services;
using FrigidRogue.MonoGame.Core.UserInterface;
using MarsUndiscovered.UserInterface.Data;

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

        public override void Handle(InterfaceRequest<GameOptionsData> request)
        {
            base.Handle(request);

            _gameOptionsStore.SaveToStore(new Memento<GameOptionsData>(Data));
        }
    }
}