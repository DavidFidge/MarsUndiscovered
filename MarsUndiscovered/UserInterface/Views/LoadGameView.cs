using System.Globalization;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Services;

using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.ViewModels;
using MarsUndiscovered.UserInterface.Data;

using FrigidRogue.MonoGame.Core.View.Extensions;

using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;

namespace MarsUndiscovered.UserInterface.Views
{
    public class LoadGameView : BaseMarsUndiscoveredView<LoadGameViewModel, LoadGameData>
    {
        private readonly ISaveGameService _saveGameService;
        private Panel _loadGamePanel;
        private SelectList _fileNameList;
        private IList<LoadGameDetails> _loadGameDetails;

        public LoadGameView(LoadGameViewModel loadGameViewModel, ISaveGameService saveGameService)
        : base(loadGameViewModel)
        {
            _saveGameService = saveGameService;
        }

        protected override void InitializeInternal()
        {
            _loadGamePanel = new Panel()
                .WidthOfContainer()
                .SolidOpacity();

            _loadGamePanel.Size = new Vector2(_loadGamePanel.Size.X, 0.8f);

            RootPanel.AddChild(_loadGamePanel);

            new Label("Choose a game to load:")
                .Centred()
                .AddTo(_loadGamePanel);

            _fileNameList = new SelectList()
                .WidthOfContainer()
                .Centred()
                .AddTo(_loadGamePanel);

            _fileNameList.Size = new Vector2(_fileNameList.Size.X, 0.95f);
            _fileNameList.OnValueChange += OnValueChange;
        }

        private void OnValueChange(Entity entity)
        {
            if (_fileNameList.SelectedIndex != -1)
            {
                Mediator.Send(new CloseLoadGameViewRequest());
                Mediator.Send(new LoadGameRequest { Filename = _loadGameDetails[_fileNameList.SelectedIndex].Filename });
            }
        }

        public override void Show()
        {
            base.Show();

            _loadGameDetails = new List<LoadGameDetails>();

            var loadGameList = _saveGameService
                .GetLoadGameList()
                .OrderByDescending(l => l.DateTime)
                .ToList();

            _fileNameList.ClearItems();

            foreach (var loadGame in loadGameList)
            {
                _fileNameList.AddItem($"{loadGame.Filename}, {loadGame.LoadGameDetail}, {loadGame.DateTime.ToString("f", CultureInfo.CurrentCulture)}");
                _loadGameDetails.Add(loadGame);
            }
        }
    }
}