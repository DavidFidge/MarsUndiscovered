using System.Threading;
using FrigidRogue.MonoGame.Core.Components.Mediator;
using FrigidRogue.MonoGame.Core.Services;
using FrigidRogue.MonoGame.Core.View.Extensions;
using GeonBit.UI.Entities;
using MarsUndiscovered.Interfaces;
using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

namespace MarsUndiscovered.UserInterface.Views
{
    public class SaveGameView : BaseMarsUndiscoveredView<SaveGameViewModel, SaveGameData>,
        IRequestHandler<SaveGameRequest>
    {
        private readonly IGameWorldEndpoint _gameWorldEndpoint;
        private Panel _saveGamePanel;
        private TextInput _saveGameName;
        private Button _overwriteButton;
        private Label _overwriteLabel;
        private Label _errorLabel;

        public SaveGameView(
            SaveGameViewModel saveGameViewModel,
            IGameWorldEndpoint gameWorldEndpoint)
            : base(saveGameViewModel)
        {
            _gameWorldEndpoint = gameWorldEndpoint;
        }

        protected override void InitializeInternal()
        {
            _saveGamePanel = new Panel()
                .AutoHeight()
                .WidthOfButtonWithPadding()
                .OpacityPercent(90);

            RootPanel.AddChild(_saveGamePanel);

            new Label("Save Game Name:")
                .AddTo(_saveGamePanel);

            _saveGameName = new TextInput()
                .WidthOfButton(TextInput.DefaultStyle.GetStyleProperty("DefaultSize").asVector.Y)
                .AddTo(_saveGamePanel);

            new Button("Save")
                .SendOnClick<SaveGameRequest>(Mediator)
                .AddTo(_saveGamePanel);

            _overwriteLabel = new Label("Save game already exists. Overwrite?")
                .Hidden()
                .AddTo(_saveGamePanel);

            _overwriteButton = new Button("Overwrite")
                .SendOnClick<SaveGameRequest>(s => s.Overwrite = true, Mediator)
                .Hidden()
                .AddTo(_saveGamePanel);

            new Button("Cancel")
                .SendOnClick<CloseSaveGameViewRequest>(Mediator)
                .AddTo(_saveGamePanel);

            _errorLabel = new Label()
                .Hidden()
                .AddTo(_saveGamePanel);
        }

        private void Reset()
        {
            _overwriteButton.Visible = false;
            _overwriteLabel.Visible = false;
            _errorLabel.Visible = false;

            _saveGamePanel.ForceDirty();
        }

        public override void Show()
        {
            Reset();

            base.Show();

            _saveGameName.Value = _gameWorldEndpoint.GetSeed();
        }

        public void Handle(SaveGameRequest request)
        {
            // We don't want the 's' key to save the game when use is entering a save game name
            if (request.FromHotkey && _saveGameName.IsFocused)
                return;

            if (_saveGameName.Value != null)
            {
                var result = _gameWorldEndpoint.SaveGame(_saveGameName.Value, request.Overwrite);

                Reset();

                if (result.Equals(SaveGameResult.Overwrite))
                {
                    _overwriteLabel.Visible = true;
                    _overwriteButton.Visible = true;
                }
                else if (!result.Equals(SaveGameResult.Success))
                {
                    _errorLabel.Visible = true;
                }
                else
                {
                    Mediator.Send(new CloseSaveGameViewRequest());
                }
            }
        }
    }
}