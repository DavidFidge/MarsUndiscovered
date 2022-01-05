using System.Threading;
using System.Threading.Tasks;
using FrigidRogue.MonoGame.Core.Services;

using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

using FrigidRogue.MonoGame.Core.View.Extensions;

using GeonBit.UI.Entities;

using MarsUndiscovered.Interfaces;
using MediatR;

namespace MarsUndiscovered.UserInterface.Views
{
    public class SaveGameView : BaseMarsUndiscoveredView<SaveGameViewModel, SaveGameData>,
        IRequestHandler<SaveGameRequest>
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

            _saveGameName.Value = _gameWorldProvider.GameWorld.Seed.ToString();
        }

        public Task<Unit> Handle(SaveGameRequest request, CancellationToken cancellationToken)
        {
            // We don't want the 's' key to save the game when use is entering a save game name
            if (request.FromHotkey && _saveGameName.IsFocused)
                return Unit.Task;

            if (_saveGameName.Value != null)
            {
                var result = _gameWorldProvider.GameWorld.SaveGame(_saveGameName.Value, request.Overwrite);

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

            return Unit.Task;
        }
    }
}