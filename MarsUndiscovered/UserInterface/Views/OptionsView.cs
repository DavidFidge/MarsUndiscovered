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
                .OpacityPercent(90);

            RootPanel.AddChild(_optionsMenuPanel);

            SetupSharedChildPanelWithButton<OpenVideoOptionsRequest, VideoOptionsViewModel, VideoOptionsData>(_optionsMenuPanel, "Video Options", _videoOptionsView);

            new Button("Game Options (not implemented yet")
                .SendOnClick<OpenGameOptionsRequest>(Mediator)
                .AddTo(_optionsMenuPanel);

            new Button("Back")
                .SendOnClick<CloseOptionsViewRequest>(Mediator)
                .AddTo(_optionsMenuPanel);
        }

        public Task<Unit> Handle(OpenVideoOptionsRequest request, CancellationToken cancellationToken)
        {
            return ShowChildViewWithRootSwap(_videoOptionsView, _optionsMenuPanel);
        }

        public Task<Unit> Handle(CloseVideoOptionsRequest request, CancellationToken cancellationToken)
        {
            return HideChildView(_videoOptionsView, _optionsMenuPanel);
        }
    }
}