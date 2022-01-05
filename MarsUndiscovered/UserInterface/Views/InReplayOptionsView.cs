using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

using FrigidRogue.MonoGame.Core.View.Extensions;

using GeonBit.UI.Entities;

namespace MarsUndiscovered.UserInterface.Views
{
    public class InReplayOptionsView : BaseMarsUndiscoveredView<InReplayOptionsViewModel, InReplayOptionsData>
    {
        private Panel _inReplayOptionsMenuPanel;

        public InReplayOptionsView(InReplayOptionsViewModel inReplayOptionsViewModel
        ) : base(inReplayOptionsViewModel)
        {
        }

        protected override void InitializeInternal()
        {
            _inReplayOptionsMenuPanel = new Panel()
                .AutoHeight()
                .WidthOfButtonWithPadding()
                .OpacityPercent(90);

            RootPanel.AddChild(_inReplayOptionsMenuPanel);

            new Button("Exit Replay")
                .SendOnClick<CloseInReplayOptionsRequest, QuitToTitleRequest>(Mediator)
                .AddTo(_inReplayOptionsMenuPanel);

            new Button("Back to Replay")
                .SendOnClick<CloseInReplayOptionsRequest>(Mediator)
                .AddTo(_inReplayOptionsMenuPanel);
        }
    }
}