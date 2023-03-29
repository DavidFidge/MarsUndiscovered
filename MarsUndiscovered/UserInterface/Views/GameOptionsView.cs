using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.MonoGame.Core.UserInterface;
using FrigidRogue.MonoGame.Core.View.Extensions;
using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;

namespace MarsUndiscovered.UserInterface.Views
{
    public class GameOptionsView :
        BaseMarsUndiscoveredView<GameOptionsViewModel, GameOptionsData>
    {
        private CheckBox _uploadMorgueCheckBox;
        private CheckBox _useAsciiTiles;
        private TextInput _morgueFileUsernameInput;

        public GameOptionsView(GameOptionsViewModel gameOptionsViewModel)
            : base(gameOptionsViewModel)
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
                    Mediator.Send(new CloseGameOptionsRequest());
                }
            };

            buttonPanel.AddChild(backButton);
        }

        private Panel CreateOptionsItemPanel(Panel container)
        {
            return new Panel()
                .SkinNone()
                .NoPadding()
                .AutoHeight()
                .WidthOfContainer()
                .Anchor(Anchor.Auto)
                .AddTo(container);
        }

        private void AddContentPanelContent(Panel contentPanel)
        {
            var morgueUsernameContainer = CreateOptionsItemPanel(contentPanel);
            
            new Label("Morgue Username")
                .Anchor(Anchor.CenterLeft)
                .NoPadding()
                .Width(500)
                .AddTo(morgueUsernameContainer);

            _morgueFileUsernameInput = new TextInput()
                .Anchor(Anchor.TopRight)
                .WidthOfButton(TextInput.DefaultStyle.GetStyleProperty("DefaultSize").asVector.Y)
                .AddTo(morgueUsernameContainer);

            _morgueFileUsernameInput.CharactersLimit = 50;
            _morgueFileUsernameInput.Value = Data.MorgueUsername;

            _morgueFileUsernameInput.OnValueChange = entity =>
            {
                Mediator.Send(new InterfaceRequest<GameOptionsData>(
                    Data.GetPropertyInfo(nameof(Data.MorgueUsername)),
                    ((TextInput)entity).TextParagraph.Text.Length < 50 
                        ? ((TextInput)entity).TextParagraph.Text 
                        : ((TextInput)entity).TextParagraph.Text.Substring(0, 50)));
            };

            _uploadMorgueCheckBox = new CheckBox("Upload Morgue Files to Website", offset: new Vector2(Constants.UiIndentLevel1, 0f))
            {
                Checked = Data.UploadMorgueFiles,
                OnValueChange = entity =>
                    Mediator.Send(new InterfaceRequest<GameOptionsData>(Data.GetPropertyInfo(nameof(Data.UploadMorgueFiles)), ((CheckBox)entity).Checked))
            };

            _uploadMorgueCheckBox.AddTo(contentPanel);

            _useAsciiTiles = new CheckBox("Use Ascii Tiles", offset: new Vector2(Constants.UiIndentLevel1, 0f))
            {
                Checked = Data.UseAsciiTiles,
                OnValueChange = entity =>
                    Mediator.Send(new InterfaceRequest<GameOptionsData>(Data.GetPropertyInfo(nameof(Data.UseAsciiTiles)), ((CheckBox)entity).Checked))
            };

            _useAsciiTiles.AddTo(contentPanel);
        }
    }
}