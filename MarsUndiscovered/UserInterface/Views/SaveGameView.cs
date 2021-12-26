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
    public class SaveGameView : BaseMarsUndiscoveredView<SaveGameViewModel, SaveGameData>
    {
        private Panel _saveGamePanel;

        public SaveGameView(SaveGameViewModel saveGameViewModel)
            : base(saveGameViewModel)
        {
        }

        protected override void InitializeInternal()
        {
            _saveGamePanel = new Panel()
                .AutoHeight()
                .WidthOfButtonWithPadding()
                .Opacity90Percent();

            RootPanel.AddChild(_saveGamePanel);

            SetupVideoOptionsItem();


            new Button("Cancel")
                .SendOnClick<CloseSaveGameViewRequest>(Mediator)
                .AddTo(_saveGamePanel);
        }
    }
}