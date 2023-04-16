using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;
using FrigidRogue.MonoGame.Core.View.Extensions;
using GeonBit.UI.Entities;

namespace MarsUndiscovered.UserInterface.Views
{
    public class DeveloperToolsView :
        BaseMarsUndiscoveredView<DeveloperToolsViewModel, DeveloperToolsData>
    {
        public DeveloperToolsView(DeveloperToolsViewModel developerToolsViewModel)
            : base(developerToolsViewModel)
        {
        }

        protected override void InitializeInternal()
        {
            var gameOptionsPanel = new Panel()
                .AutoHeight()
                .Width(1200)
                .SolidOpacity();

            RootPanel.AddChild(gameOptionsPanel);

            AddContentPanelContent(gameOptionsPanel);
            AddButtonPanelContent(gameOptionsPanel);
        }

        private void AddButtonPanelContent(Panel buttonPanel)
        {
            var backButton = new Button("Back", ButtonSkin.Default, Anchor.AutoCenter)
            {
                OnClick = entity =>
                {
                    Mediator.Send(new CloseDeveloperToolsViewRequest());
                }
            };

            buttonPanel.AddChild(backButton);
        }

        private void AddContentPanelContent(Panel contentPanel)
        {
            new Button("World Builder")
                .SendOnClick<WorldBuilderRequest>(Mediator)
                .AddTo(contentPanel);

            new Button("Wave Function Collapse")
                .SendOnClick<WaveFunctionCollapseRequest>(Mediator)
                .AddTo(contentPanel);
        }
    }
}