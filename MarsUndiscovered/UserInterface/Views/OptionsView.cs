using System.Threading;
using System.Threading.Tasks;

using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

using FrigidRogue.MonoGame.Core.View.Extensions;

using GeonBit.UI.Entities;

using MediatR;
using SharpDX.MediaFoundation;

namespace MarsUndiscovered.UserInterface.Views
{
    public class OptionsView : BaseMarsUndiscoveredView<OptionsViewModel, OptionsData>,
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
            _optionsMenuPanel = new Panel()
                .AutoHeight()
                .WidthOfButtonWithPadding()
                .Opacity90Percent();

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

            // Only initialize, do not add as child - this is done when handling the message
            // as it is a shared component between this and in game options screen.
            _videoOptionsView.Initialize();
        }

        public Task<Unit> Handle(OpenVideoOptionsRequest request, CancellationToken cancellationToken)
        {
            _videoOptionsView.RootPanel.ClearParent();
            RootPanel.AddChild(_videoOptionsView.RootPanel);
            _videoOptionsView.Show();
            _optionsMenuPanel.Visible = false;

            return Unit.Task;
        }

        public Task<Unit> Handle(CloseVideoOptionsRequest request, CancellationToken cancellationToken)
        {
            if (RootPanel.HasChild(_videoOptionsView.RootPanel))
            {
                _videoOptionsView.Hide();
                _optionsMenuPanel.Visible = true;
            }

            return Unit.Task;
        }
    }
}