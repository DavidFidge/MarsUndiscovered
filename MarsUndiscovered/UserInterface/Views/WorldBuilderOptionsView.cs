using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

using FrigidRogue.MonoGame.Core.View.Extensions;

using GeonBit.UI.Entities;

namespace MarsUndiscovered.UserInterface.Views
{
    public class WorldBuilderOptionsView : BaseMarsUndiscoveredView<WorldBuilderOptionsViewModel, WorldBuilderOptionsData>
    {
        private Panel _worldBuilderOptionsMenuPanel;

        public WorldBuilderOptionsView(WorldBuilderOptionsViewModel worldBuilderOptionsViewModel
        ) : base(worldBuilderOptionsViewModel)
        {
        }

        protected override void InitializeInternal()
        {
            _worldBuilderOptionsMenuPanel = new Panel()
                .AutoHeight()
                .WidthOfButtonWithPadding()
                .OpacityPercent(90);

            RootPanel.AddChild(_worldBuilderOptionsMenuPanel);

            new Button("Exit World Builder")
                .SendOnClick<CloseWorldBuilderOptionsRequest, QuitToTitleRequest>(Mediator)
                .AddTo(_worldBuilderOptionsMenuPanel);

            new Button("Back to World Builder")
                .SendOnClick<CloseWorldBuilderOptionsRequest>(Mediator)
                .AddTo(_worldBuilderOptionsMenuPanel);
        }
    }
}