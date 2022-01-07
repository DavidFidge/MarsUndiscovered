using System.Linq;

using FrigidRogue.MonoGame.Core.Graphics.Camera;
using FrigidRogue.MonoGame.Core.View.Extensions;

using GeonBit.UI.Entities;

using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MarsUndiscovered.UserInterface.Views
{
    public abstract class BaseGameView<TViewModel, TData> : BaseMarsUndiscoveredView<TViewModel, TData>
        where TViewModel : BaseGameViewModel<TData>
        where TData : BaseGameData, new()
    {
        protected readonly IGameCamera _gameCamera;
        public bool IsMouseInGameView => RootPanel?.IsMouseInRootPanelEmptySpace ?? true;
        private SelectList _messageLog;
        protected Panel _leftPanel;
        protected ProgressBar _playerHealthBar;

        protected BaseGameView(IGameCamera gameCamera, TViewModel viewModel) : base(viewModel)
        {
            _gameCamera = gameCamera;
        }

        protected void CreateLeftPanel()
        {
            _leftPanel = new Panel()
                .Anchor(Anchor.TopLeft)
                .Width(0.19f)
                .NoSkin()
                .NoPadding()
                .Height(0.999f);

            RootPanel.AddChild(_leftPanel);
        }

        protected void CreatePlayerPanel()
        {
            var playerPanel = new Panel()
                .NoSkin()
                .NoPadding()
                .WidthOfScreen()
                .Anchor(Anchor.AutoInline);

            _leftPanel.AddChild(playerPanel);

            _playerHealthBar = new ProgressBar()
                .NoPadding()
                .Anchor(Anchor.TopLeft)
                .TransparentFillColor()
                .ProgressBarFillColor(Color.Red)
                .Locked();

            var healthLabel = new Label("Health")
                .NoPadding()
                .Anchor(Anchor.Center);

            _playerHealthBar.AddChild(healthLabel);

            playerPanel.AddChild(_playerHealthBar);
        }

        public override void Draw()
        {
            var oldDepthStencilState = Game.GraphicsDevice.DepthStencilState;
            Game.GraphicsDevice.DepthStencilState = DepthStencilState.None;

            _viewModel.SceneGraph.Draw(_gameCamera.View, _gameCamera.Projection);

            Game.GraphicsDevice.DepthStencilState = oldDepthStencilState;

            base.Draw();
        }

        public override void Update()
        {
            var newMessages = _viewModel.GetNewMessages();

            if (newMessages.Any())
            {
                foreach (var message in newMessages)
                    _messageLog.AddItem(message);

                _messageLog.scrollToEnd();
            }

            _gameCamera.Update();

            base.Update();
        }

        protected void AddMessageLog()
        {
            _messageLog = new SelectList(new Vector2(0.61f, 0.14f), Anchor.TopCenter, null, PanelSkin.None)
                .NoPadding();

            _messageLog.ExtraSpaceBetweenLines = -10;
            _messageLog.LockSelection = true;
            RootPanel.AddChild(_messageLog);

            _messageLog.OnListChange = entity =>
            {
                var list = (SelectList)entity;
                if (list.Count > 100)
                    list.RemoveItem(0);
            };
        }

        protected virtual void ResetViews()
        {
            _messageLog.ClearItems();
        }

        protected override void ViewModelChanged()
        {
            base.ViewModelChanged();

            _playerHealthBar.Max = (uint)_viewModel.PlayerMaxHealth;
            _playerHealthBar.StepsCount = _playerHealthBar.Max;
            _playerHealthBar.Value = _viewModel.PlayerHealth;
        }
    }
}