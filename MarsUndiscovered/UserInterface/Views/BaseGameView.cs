using System;
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
        protected Panel LeftPanel;
        protected Panel BottomPanel;
        protected Panel TopPanel;
        protected PlayerPanel PlayerPanel;
        protected IList<MonsterPanel> MonsterPanels = new List<MonsterPanel>();
        protected RichParagraph StatusParagraph;

        protected BaseGameView(IGameCamera gameCamera, TViewModel viewModel) : base(viewModel)
        {
            _gameCamera = gameCamera;
        }

        protected void CreateLayoutPanels()
        {
            LeftPanel = new Panel()
                .Anchor(Anchor.TopLeft)
                .Width(0.19f)
                .NoSkin()
                .NoPadding()
                .Height(0.999f);

            RootPanel.AddChild(LeftPanel);

            TopPanel = new Panel()
                .Anchor(Anchor.TopCenter)
                .Width(Constants.MiddlePanelWidth)
                .NoSkin()
                .NoPadding()
                .Height(0.14f);

            RootPanel.AddChild(TopPanel);

            BottomPanel = new Panel()
                .Anchor(Anchor.BottomCenter)
                .Width(Constants.MiddlePanelWidth)
                .NoSkin()
                .NoPadding()
                .Height(0.1f);

            RootPanel.AddChild(BottomPanel);
        }

        protected void CreateStatusPanel()
        {
            StatusParagraph = new RichParagraph()
                .Anchor(Anchor.BottomCenter)
                .NoPadding()
                .Height(0.1f);

            BottomPanel.AddChild(StatusParagraph);
        }

        protected void CreateMessageLog()
        {
            _messageLog = new SelectList()
                .NoSkin()
                .Anchor(Anchor.Auto)
                .NoPadding();

            _messageLog.ExtraSpaceBetweenLines = -10;
            _messageLog.LockSelection = true;
            TopPanel.AddChild(_messageLog);

            _messageLog.OnListChange = entity =>
            {
                var list = (SelectList)entity;
                if (list.Count > 100)
                    list.RemoveItem(0);
            };
        }

        protected void CreatePlayerPanel()
        {
            PlayerPanel = new PlayerPanel();
            PlayerPanel.AddAsChildTo(LeftPanel);
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


        protected virtual void ResetViews()
        {
            _messageLog.ClearItems();
        }

        protected override void ViewModelChanged()
        {
            base.ViewModelChanged();
            UpdateMonsterStatus();
            UpdateMessageLog();
            UpdatePlayerStatus();
            StatusParagraph.Text = String.Empty;
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
            PlayerPanel.Update(_viewModel.PlayerStatus);
        }

        protected void UpdateMonsterStatus()
        {
            foreach (var panel in MonsterPanels)
            {
                panel.RemoveFromParent();
            }

            MonsterPanels.Clear();

            var monsters = _viewModel.MonsterStatusInView
                .OrderBy(m => m.DistanceFromPlayer)
                .ToList();

            foreach (var monster in monsters)
            {
                var monsterPanel = new MonsterPanel(monster);
                MonsterPanels.Add(monsterPanel);
                monsterPanel.AddAsChildTo(LeftPanel);
            }
        }

        protected string DelimitWithDashes(string text)
        {
            return $"--- {text} ---";
        }
    }
}