using System.Linq;
using FrigidRogue.MonoGame.Core.Graphics.Camera;
using FrigidRogue.MonoGame.Core.UserInterface;
using FrigidRogue.MonoGame.Core.View;
using FrigidRogue.MonoGame.Core.View.Extensions;
using GeonBit.UI.Entities;
using MarsUndiscovered.Interfaces;
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
        public IAssets Assets { get; set; }
        public bool IsMouseInGameView => RootPanel?.IsMouseInRootPanelEmptySpace ?? true;
        private SelectList _messageLog;

        protected BaseGameView(IGameCamera gameCamera, TViewModel viewModel) : base(viewModel)
        {
            _gameCamera = gameCamera;
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
    }
}