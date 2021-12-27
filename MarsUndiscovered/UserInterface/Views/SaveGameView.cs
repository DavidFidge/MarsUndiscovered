using FrigidRogue.MonoGame.Core.Services;

using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

using FrigidRogue.MonoGame.Core.View.Extensions;

using GeonBit.UI.Entities;

using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.UserInterface.Views
{
    public class SaveGameView : BaseMarsUndiscoveredView<SaveGameViewModel, SaveGameData>
    {
        private readonly IGameWorldProvider _gameWorldProvider;
        private Panel _saveGamePanel;
        private TextInput _saveGameName;
        private Button _overwriteButton;
        private Label _overwriteLabel;
        private Label _errorLabel;

        public SaveGameView(
            SaveGameViewModel saveGameViewModel,
            IGameWorldProvider gameWorldProvider)
            : base(saveGameViewModel)
        {
            _gameWorldProvider = gameWorldProvider;
        }

        protected override void InitializeInternal()
        {
            _saveGamePanel = new Panel()
                .AutoHeight()
                .WidthOfButtonWithPadding()
                .Opacity90Percent();

            RootPanel.AddChild(_saveGamePanel);

            new Label("Save Game Name:")
                .AddTo(_saveGamePanel);

            _saveGameName = new TextInput()
                .WidthOfButton(TextInput.DefaultStyle.GetStyleProperty("DefaultSize").asVector.Y)
                .AddTo(_saveGamePanel);

            var saveGameButton = new Button("Save")
                .AddTo(_saveGamePanel);

            saveGameButton.OnClick += OnSaveGameClicked;

            _overwriteLabel = new Label("Save game already exists. Overwrite?")
                .Hidden()
                .AddTo(_saveGamePanel);

            _overwriteButton = new Button("Overwrite")
                .Hidden()
                .AddTo(_saveGamePanel);

            _overwriteButton.OnClick += OnOverwriteClicked;

            new Button("Cancel")
                .SendOnClick<CloseSaveGameViewRequest>(Mediator)
                .AddTo(_saveGamePanel);

            _errorLabel = new Label()
                .Hidden()
                .AddTo(_saveGamePanel);
        }

        private void OnOverwriteClicked(Entity entity)
        {
            if (_saveGameName.Value != null)
            {
                var result = _gameWorldProvider.GameWorld.SaveGame(_saveGameName.Value, true);

                Reset();

                if (!result.Equals(SaveGameResult.Success))
                {
                    _errorLabel.Visible = true;
                }
                else
                {
                    Mediator.Send(new CloseSaveGameViewRequest());
                }
            }
        }

        private void OnSaveGameClicked(Entity entity)
        {
            if (_saveGameName.Value != null)
            {
                var result = _gameWorldProvider.GameWorld.SaveGame(_saveGameName.Value, false);

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

        private void Reset()
        {
            _overwriteButton.Visible = false;
            _overwriteLabel.Visible = false;
            _errorLabel.Visible = false;
        }
    }
}