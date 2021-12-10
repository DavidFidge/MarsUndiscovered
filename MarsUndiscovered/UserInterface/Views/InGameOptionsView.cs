using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

using FrigidRogue.MonoGame.Core.View;
using FrigidRogue.MonoGame.Core.View.Extensions;

using GeonBit.UI.Entities;

using Microsoft.Xna.Framework;

namespace MarsUndiscovered.UserInterface.Views
{
    public class InGameOptionsView : BaseMarsUndiscoveredView<InGameOptionsViewModel, InGameOptionsData>
    {
        private Panel _inGameOptionsMenuPanel;

        public InGameOptionsView(
            InGameOptionsViewModel inGameOptionsViewModel
        ) : base(inGameOptionsViewModel)
        {
        }

        protected override void InitializeInternal()
        {
            _inGameOptionsMenuPanel = new Panel(new Vector2(500, 400));

            RootPanel.AddChild(_inGameOptionsMenuPanel);

            var headingLabel = new Label(Data.Heading, Anchor.AutoCenter)
                .H4Heading();

            _inGameOptionsMenuPanel.AddChild(headingLabel);

            var line = new HorizontalLine(Anchor.AutoCenter);

            _inGameOptionsMenuPanel.AddChild(line);

            new Button("Exit Game")
                .SendOnClick<CloseInGameOptionsRequest, QuitToTitleRequest>(Mediator)
                .AddTo(_inGameOptionsMenuPanel);

            new Button("Back to Game")
                .SendOnClick<CloseInGameOptionsRequest>(Mediator)
                .AddTo(_inGameOptionsMenuPanel);
        }
    }
}