using System.Threading;
using System.Threading.Tasks;

using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

using FrigidRogue.MonoGame.Core.View.Extensions;

using GeonBit.UI.Entities;

using MediatR;

namespace MarsUndiscovered.UserInterface.Views
{
    public class OptionsView : BaseMarsUndiscoveredView<OptionsViewModel, OptionsData>,
        IRequestHandler<OpenVideoOptionsRequest>,
        IRequestHandler<CloseVideoOptionsRequest>,
        IRequestHandler<OpenGameOptionsRequest>,
        IRequestHandler<CloseGameOptionsRequest>
    {
        private readonly VideoOptionsView _videoOptionsView;
        private readonly GameOptionsView _gameOptionsView;
        private Panel _optionsMenuPanel;

        public OptionsView(
            OptionsViewModel optionsViewModel,
            VideoOptionsView videoOptionsView,
            GameOptionsView gameOptionsView
            )
        
        : base(optionsViewModel)
        {
            _videoOptionsView = videoOptionsView;
            _gameOptionsView = gameOptionsView;
        }

        protected override void InitializeInternal()
        {
            _optionsMenuPanel = new Panel()
                .AutoHeight()
                .WidthOfButtonWithPadding()
                .OpacityPercent(90);

            RootPanel.AddChild(_optionsMenuPanel);

            SetupSharedChildPanelWithButton<OpenVideoOptionsRequest, VideoOptionsViewModel, VideoOptionsData>(_optionsMenuPanel, "Video Options", _videoOptionsView);

            SetupChildPanelWithButton<OpenGameOptionsRequest, GameOptionsViewModel, GameOptionsData>(
                _optionsMenuPanel,
                "Game Options",
                _gameOptionsView);
            
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
        
        public Task<Unit> Handle(OpenGameOptionsRequest request, CancellationToken cancellationToken)
        {
            return ShowChildView(_gameOptionsView, _optionsMenuPanel);
        }

        public Task<Unit> Handle(CloseGameOptionsRequest request, CancellationToken cancellationToken)
        {
            return HideChildView(_gameOptionsView, _optionsMenuPanel);
        }
    }
}