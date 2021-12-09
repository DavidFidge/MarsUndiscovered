using System.Threading;
using System.Threading.Tasks;

using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

using FrigidRogue.MonoGame.Core.View;
using FrigidRogue.MonoGame.Core.View.Extensions;

using GeonBit.UI.Entities;

using MediatR;

namespace MarsUndiscovered.UserInterface.Views
{
    public class OptionsView : BaseView<OptionsViewModel, OptionsData>,
        IRequestHandler<OpenVideoOptionsRequest>,
        IRequestHandler<CloseVideoOptionsRequest>
    {
        private readonly VideoOptionsView _videoOptionsView;
        private Panel _optionsMenuPanel;

        public OptionsView(
            OptionsViewModel optionsViewModel,
            VideoOptionsView videoOptionsView)
        : base(optionsViewModel)
        {
            _videoOptionsView = videoOptionsView;
        }

        protected override void InitializeInternal()
        {
            _optionsMenuPanel = new Panel();
            _optionsMenuPanel.AdjustHeightAutomatically = true;
            _optionsMenuPanel.Opacity = 200;

            RootPanel.AddChild(_optionsMenuPanel);

            SetupVideoOptionsItem();

            new Button("Game Options (not implemented yet")
                .SendOnClick<OpenGameOptionsRequest>(Mediator)
                .AddTo(_optionsMenuPanel);

            new Button("Back")
                .SendOnClick<CloseOptionsViewRequest>(Mediator)
                .AddTo(_optionsMenuPanel);
        }

        private void SetupVideoOptionsItem()
        {
            new Button("Video Options")
                .SendOnClick<OpenVideoOptionsRequest>(Mediator)
                .AddTo(_optionsMenuPanel);

            _videoOptionsView.Initialize();
            
            RootPanel.AddChild(_videoOptionsView.RootPanel);
        }

        public Task<Unit> Handle(OpenVideoOptionsRequest request, CancellationToken cancellationToken)
        {
            _videoOptionsView.Show();
            _optionsMenuPanel.Visible = false;

            return Unit.Task;
        }

        public Task<Unit> Handle(CloseVideoOptionsRequest request, CancellationToken cancellationToken)
        {
            _videoOptionsView.Hide();
            _optionsMenuPanel.Visible = true;

            return Unit.Task;
        }
    }
}