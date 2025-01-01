using System.Threading;
using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.View.Extensions;
using GeonBit.UI.Entities;
using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

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

            SetupSharedChildPanelWithButton<OpenGameOptionsRequest, GameOptionsViewModel, GameOptionsData>(_optionsMenuPanel, "Game Options", _gameOptionsView);
            
            new Button("Back")
                .SendOnClick<CloseOptionsViewRequest>(Mediator)
                .AddTo(_optionsMenuPanel);
        }

        public void Handle(OpenVideoOptionsRequest request)
        {
            ShowChildViewWithRootSwap(_videoOptionsView, _optionsMenuPanel);
        }

        public void Handle(CloseVideoOptionsRequest request)
        {
            HideChildView(_videoOptionsView, _optionsMenuPanel);
        }
        
        public void Handle(OpenGameOptionsRequest request)
        {
            ShowChildViewWithRootSwap(_gameOptionsView, _optionsMenuPanel);
        }

        public void Handle(CloseGameOptionsRequest request)
        {
            HideChildView(_gameOptionsView, _optionsMenuPanel);
        }
    }
}