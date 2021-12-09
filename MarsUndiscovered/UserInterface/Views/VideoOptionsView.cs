using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.MonoGame.Core.Services;
using FrigidRogue.MonoGame.Core.UserInterface;
using FrigidRogue.MonoGame.Core.View;

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
            var videoOptionsPanel = new Panel();
            videoOptionsPanel.AdjustHeightAutomatically = true;
            videoOptionsPanel.Opacity = 200;

            RootPanel.AddChild(videoOptionsPanel);

            AddContentPanelContent(videoOptionsPanel);
            AddButtonPanelContent(videoOptionsPanel);
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
            var displayModesLabel = new Label(LabelFor(() => Data.DisplayDimensions));
            var displayModesDropDown = new DropDown<DisplayDimension>(Data.DisplayDimensions);
            displayModesDropDown.SelectedIndex = Data.DisplayDimensions.IndexOf(Data.SelectedDisplayDimension);
            displayModesDropDown.AutoSetListHeight = true;

            var renderResolutionLabel = new Label(LabelFor(() => Data.RenderResolutions));
            var renderResolutionDropDown = new DropDown<RenderResolution>(Data.RenderResolutions);
            renderResolutionDropDown.SelectedIndex = Data.RenderResolutions.IndexOf(Data.SelectedRenderResolution);
            renderResolutionDropDown.AutoSetListHeight = true;

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

            contentPanel.AddChild(displayModesLabel);
            contentPanel.AddChild(displayModesDropDown);
            contentPanel.AddChild(renderResolutionLabel);
            contentPanel.AddChild(renderResolutionDropDown);
            contentPanel.AddChild(fullScreenCheckbox);
            contentPanel.AddChild(isBorderlessWindowedCheckBox);

            var verticalSyncCheckbox = new CheckBox("Vertical Sync")
            {
                Checked = Data.IsVerticalSync,
                OnValueChange = entity =>
                    Mediator.Send(new InterfaceRequest<VideoOptionsData>(Data.GetPropertyInfo(nameof(Data.IsVerticalSync)), ((CheckBox)entity).Checked))
            };

            contentPanel.AddChild(verticalSyncCheckbox);

            displayModesDropDown.OnValueChange = entity =>
            {
                var request = new SetDisplayModeRequest(
                    ((DropDown<DisplayDimension>)entity).SelectedValueTyped
                );

                Mediator.Send(request);
            };

            renderResolutionDropDown.OnValueChange = entity =>
            {
                var request = new SetRenderResolutionRequest(
                    ((DropDown<RenderResolution>)entity).SelectedValueTyped
                );

                Mediator.Send(request);
            };
        }
    }
}