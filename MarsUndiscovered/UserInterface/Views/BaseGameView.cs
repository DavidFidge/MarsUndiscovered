using System.Collections.Generic;
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
        private PlayerPanel _playerPanel;
        private IList<MonsterPanel> _monsterPanels = new List<MonsterPanel>();

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
            _playerPanel = new PlayerPanel();
            _playerPanel.AddAsChildTo(_leftPanel);
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
            UpdatePlayerStatus();
            UpdateMonsterStatus();
            UpdateMessageLog();
        }

        private void UpdateMessageLog()
        {
            var newMessages = _viewModel.Messages;

            if (newMessages.Any())
            {
                foreach (var message in newMessages)
                    _messageLog.AddItem(message);

                _messageLog.scrollToEnd();
            }
        }

        private void UpdatePlayerStatus()
        {
            _playerPanel.Update(_viewModel.PlayerStatus);
        }

        protected void UpdateMonsterStatus()
        {
            foreach (var panel in _monsterPanels)
            {
                panel.RemoveFromParent();
            }

            _monsterPanels.Clear();

            var monsters = _viewModel.MonsterStatusInView
                .OrderBy(m => m.DistanceFromPlayer)
                .ToList();

            foreach (var monster in monsters)
            {
                var monsterPanel = new MonsterPanel(monster);
                _monsterPanels.Add(monsterPanel);
                monsterPanel.AddAsChildTo(_leftPanel);
            }
        }
    }
}