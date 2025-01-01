using FrigidRogue.MonoGame.Core.UserInterface;
using FrigidRogue.MonoGame.Core.View;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.UserInterface.Views
{
    public abstract class BaseMarsUndiscoveredView<TViewModel, TData> : BaseView<TViewModel, TData>
        where TViewModel : BaseViewModel<TData>
        where TData : new ()
    {
        public IAssets Assets { get; set; }

        protected BaseMarsUndiscoveredView(TViewModel viewModel) : base(viewModel)
        {
        }
    }
}