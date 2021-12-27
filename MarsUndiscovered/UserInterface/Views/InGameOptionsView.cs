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
    public class InGameOptionsView : BaseMarsUndiscoveredView<InGameOptionsViewModel, InGameOptionsData>,
        IRequestHandler<OpenInGameVideoOptionsRequest>,
        IRequestHandler<CloseInGameVideoOptionsRequest>,
        IRequestHandler<OpenSaveGameViewRequest>,
        IRequestHandler<CloseSaveGameViewRequest>
    {
        private readonly VideoOptionsView _videoOptionsView;
        private readonly SaveGameView _saveGameView;
        private Panel _inGameOptionsMenuPanel;

        public InGameOptionsView(
            InGameOptionsViewModel inGameOptionsViewModel,
            VideoOptionsView videoOptionsView,
            SaveGameView saveGameView
        ) : base(inGameOptionsViewModel)
        {
            _videoOptionsView = videoOptionsView;
            _saveGameView = saveGameView;
        }

        protected override void InitializeInternal()
        {
            _inGameOptionsMenuPanel = new Panel()
                .AutoHeight()
                .WidthOfButtonWithPadding()
                .Opacity90Percent();

            RootPanel.AddChild(_inGameOptionsMenuPanel);

            SetupSaveGameItem();

            SetupVideoOptionsItem();

            new Button("Exit Game")
                .SendOnClick<CloseInGameOptionsRequest, QuitToTitleRequest>(Mediator)
                .AddTo(_inGameOptionsMenuPanel);

            new Button("Back to Game")
                .SendOnClick<CloseInGameOptionsRequest>(Mediator)
                .AddTo(_inGameOptionsMenuPanel);
        }

        private void SetupVideoOptionsItem()
        {
            new Button("Video Options")
                .SendOnClick<OpenInGameVideoOptionsRequest>(Mediator)
                .AddTo(_inGameOptionsMenuPanel);

            // Only initialize, do not add as child - this is done when handling the message
            // as it is a shared component between this and title options screen.
            _videoOptionsView.Initialize();
        }

        private void SetupSaveGameItem()
        {
            new Button("Save Game")
                .SendOnClick<OpenSaveGameViewRequest>(Mediator)
                .AddTo(_inGameOptionsMenuPanel);

            _saveGameView.Initialize();

            RootPanel.AddChild(_saveGameView.RootPanel);
        }

        public Task<Unit> Handle(OpenSaveGameViewRequest request, CancellationToken cancellationToken)
        {
            _saveGameView.Show();
            _inGameOptionsMenuPanel.Visible = false;

            return Unit.Task;
        }

        public Task<Unit> Handle(CloseSaveGameViewRequest request, CancellationToken cancellationToken)
        {
            _saveGameView.Hide();
            _inGameOptionsMenuPanel.Visible = true;

            return Unit.Task;
        }

        public Task<Unit> Handle(OpenInGameVideoOptionsRequest request, CancellationToken cancellationToken)
        {
            _videoOptionsView.RootPanel.ClearParent();
            RootPanel.AddChild(_videoOptionsView.RootPanel);
            _videoOptionsView.Show();
            _inGameOptionsMenuPanel.Visible = false;

            return Unit.Task;
        }

        public Task<Unit> Handle(CloseInGameVideoOptionsRequest request, CancellationToken cancellationToken)
        {
            if (RootPanel.HasChild(_videoOptionsView.RootPanel))
            {
                _videoOptionsView.Hide();
                _inGameOptionsMenuPanel.Visible = true;
            }

            return Unit.Task;
        }
    }
}