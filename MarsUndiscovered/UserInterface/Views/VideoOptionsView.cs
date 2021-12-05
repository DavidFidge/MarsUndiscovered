using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.MonoGame.Core.Services;
using FrigidRogue.MonoGame.Core.UserInterface;
using FrigidRogue.MonoGame.Core.View;
using FrigidRogue.MonoGame.Core.View.Extensions;

using GeonBit.UI.Entities;

using Microsoft.Xna.Framework;

namespace MarsUndiscovered.UserInterface.Views
{
    public class VideoOptionsView : 
        BaseView<VideoOptionsViewModel, VideoOptionsData>
    {
        public VideoOptionsView(VideoOptionsViewModel videoOptionsViewModel)
            : base(videoOptionsViewModel)
        {
        }

        protected override void InitializeInternal()
        {
            var containerPanel = new Panel(new Vector2(0, 0), PanelSkin.None)
            {
                Padding = new Vector2(20f, 20f)
            };

            RootPanel.AddChild(containerPanel);

            var videoOptionsPanel = new Panel(new Vector2(0, 0))
            {
                Padding = new Vector2(30f, 30f)
            };

            containerPanel.AddChild(videoOptionsPanel);

            var headingLabel = new Label(Data.Heading, Anchor.AutoCenter)
                .H3Heading();

            var line = new HorizontalLine(Anchor.AutoCenter);

            var contentPanel = new Panel(new Vector2(1200f, 0.5f), PanelSkin.None, Anchor.AutoCenter);
            contentPanel.Padding = new Vector2(0, 0);

            var buttonPanel = new Panel(new Vector2(500f, 100f), PanelSkin.None, Anchor.BottomCenter);

            videoOptionsPanel.AddChild(headingLabel);
            videoOptionsPanel.AddChild(line);
            videoOptionsPanel.AddChild(contentPanel);
            videoOptionsPanel.AddChild(buttonPanel);

            AddContentPanelContent(contentPanel);
            AddButtonPanelContent(buttonPanel);
        }

        private void AddButtonPanelContent(Panel buttonPanel)
        {
            var backButton = new Button("Back", ButtonSkin.Default, Anchor.AutoCenter)
            {
                OnClick = entity =>
                {
                    Mediator.Send(new SaveVideoOptionsRequest());
                    Mediator.Send(new CloseVideoOptionsRequest());
                }
            };

            buttonPanel.AddChild(backButton);
        }

        private void AddContentPanelContent(Panel contentPanel)
        {
            var leftPanel = new Panel(new Vector2(600f, 1f), PanelSkin.None, Anchor.AutoInline);

            var rightPanel = new Panel(new Vector2(600f, 1f), PanelSkin.None, Anchor.AutoInline);

            contentPanel.AddChild(leftPanel);
            contentPanel.AddChild(rightPanel);

            AddLeftPanelContent(leftPanel);
        }

        private void AddLeftPanelContent(Panel leftPanel)
        {
            var displayModesLabel = new Label(LabelFor(() => Data.DisplayModes))
                .H4Heading();

            leftPanel.AddChild(displayModesLabel);

            var displayModesSelectList = new SelectList<DisplayDimensions>(
                Data.DisplayModes,
                new Vector2(500, 500)
            );

            displayModesSelectList.SelectedIndex = Data.DisplayModes.IndexOf(Data.SelectedDisplayDimensions);

            var isBorderlessWindowedCheckBox = new CheckBox("Borderless Windowed", offset: new Vector2(Constants.UiIndentLevel1, 0f))
            {
                Checked = Data.IsBorderlessWindowed,
                OnValueChange = entity =>
                    Mediator.Send(new InterfaceRequest<VideoOptionsData>(Data.GetPropertyInfo(nameof(Data.IsBorderlessWindowed)), ((CheckBox)entity).Checked))
            };

            var fullScreenCheckbox = new CheckBox("Full Screen")
            {
                Checked = Data.IsFullScreen,
                OnValueChange = entity =>
                {
                    var isFullScreen = ((CheckBox)entity).Checked;

                    Mediator.Send(
                        new InterfaceRequest<VideoOptionsData>(
                            Data.GetPropertyInfo(nameof(Data.IsFullScreen)),
                            isFullScreen
                        )
                    );
                }
            };

            leftPanel.AddChild(displayModesSelectList);
            leftPanel.AddChild(fullScreenCheckbox);
            leftPanel.AddChild(isBorderlessWindowedCheckBox);

            var verticalSyncCheckbox = new CheckBox("Vertical Sync")
            {
                Checked = Data.IsVerticalSync,
                OnValueChange = entity =>
                    Mediator.Send(new InterfaceRequest<VideoOptionsData>(Data.GetPropertyInfo(nameof(Data.IsVerticalSync)), ((CheckBox)entity).Checked))
            };

            leftPanel.AddChild(verticalSyncCheckbox);

            displayModesSelectList.OnValueChange = entity =>
            {
                var request = new SetDisplayModeRequest(
                    ((SelectList<DisplayDimensions>)entity).SelectedValueTyped
                );

                Mediator.Send(request);
            };
        }
    }
}