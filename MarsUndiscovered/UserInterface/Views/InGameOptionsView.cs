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
        IRequestHandler<CloseInGameVideoOptionsRequest>
    {
        private readonly VideoOptionsView _videoOptionsView;
        private Panel _inGameOptionsMenuPanel;

        public InGameOptionsView(
            InGameOptionsViewModel inGameOptionsViewModel,
            VideoOptionsView videoOptionsView
        ) : base(inGameOptionsViewModel)
        {
            _videoOptionsView = videoOptionsView;
        }

        protected override void InitializeInternal()
        {
            _inGameOptionsMenuPanel = new Panel()
                .AutoHeight()
                .WidthOfButtonWithPadding()
                .Opacity90Percent();

            RootPanel.AddChild(_inGameOptionsMenuPanel);

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

            _videoOptionsView.Initialize();
        }

        private void SetupSaveGameItem()
        {
            new Button("Save Game")
                .SendOnClick<OpenInGameVideoOptionsRequest>(Mediator)
                .AddTo(_inGameOptionsMenuPanel);

            _videoOptionsView.Initialize();
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