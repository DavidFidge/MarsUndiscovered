using System.Threading;
using System.Threading.Tasks;

using FrigidRogue.MonoGame.Core.Graphics.Camera;
using FrigidRogue.MonoGame.Core.View.Extensions;

using GeonBit.UI.Entities;
using MarsUndiscovered.Messages;
using MarsUndiscovered.UserInterface.Data;
using MarsUndiscovered.UserInterface.ViewModels;

using MediatR;

using Microsoft.Xna.Framework;
using SadRogue.Primitives;
using Color = Microsoft.Xna.Framework.Color;

namespace MarsUndiscovered.UserInterface.Views
{
    public abstract class BaseGameView<TViewModel, TData> : BaseGameCoreView<TViewModel, TData>,
        INotificationHandler<MouseHoverViewNotification>
        where TViewModel : BaseGameViewModel<TData>
        where TData : BaseGameData, new()
    {
        private SelectList _messageLog;
        protected Panel LeftPanel;
        protected Panel BottomPanel;
        protected Panel TopPanel;
        protected PlayerPanel PlayerPanel;
        protected IList<MonsterPanel> MonsterPanels = new List<MonsterPanel>();
        protected RichParagraph StatusParagraph;
        protected RichParagraph HoverPanelLeftTooltip;
        protected RichParagraph HoverPanelRightTooltip;
        protected Panel GameViewPanel { get; set; }
        protected Panel HoverPanelLeft { get; set; }
        protected Panel HoverPanelRight { get; set; }

        protected BaseGameView(IGameCamera gameCamera, TViewModel viewModel) : base(gameCamera, viewModel)
        {
        }

        protected void CreateLayoutPanels()
        {
            LeftPanel = new Panel()
                .Anchor(Anchor.TopLeft)
                .Width(Constants.LeftPanelWidth)
                .SkinNone()
                .NoPadding()
                .Height(0.999f);

            RootPanel.AddChild(LeftPanel);

            TopPanel = new Panel()
                .Anchor(Anchor.TopRight)
                .Width(Constants.MiddlePanelWidth)
                .SkinNone()
                .NoPadding()
                .Offset(new Vector2(20f, 0))
                .Height(0.14f);

            RootPanel.AddChild(TopPanel);

            BottomPanel = new Panel()
                .Anchor(Anchor.BottomRight)
                .Width(Constants.MiddlePanelWidth)
                .SkinNone()
                .NoPadding()
                .Offset(new Vector2(20f, 0))
                .Height(0.1f);

            RootPanel.AddChild(BottomPanel);

            GameViewPanel = new Panel()
                .Anchor(Anchor.CenterRight)
                .Width(Constants.MiddlePanelWidth)
                .SkinNone()
                .NoPadding()
                .Height(0.79f)
                .Offset(new Vector2(20f, 120f));

            HoverPanelLeft = new Panel()
                .Anchor(Anchor.TopLeft)
                .Width(0.45f)
                .Skin(PanelSkin.Alternative)
                .AutoHeight()
                .Hidden();

            GameViewPanel.AddChild(HoverPanelLeft);

            HoverPanelLeftTooltip = new RichParagraph();

            HoverPanelLeft.AddChild(HoverPanelLeftTooltip);

            HoverPanelRight = new Panel()
                .Anchor(Anchor.TopRight)
                .Width(0.45f)
                .Skin(PanelSkin.Alternative)
                .AutoHeight()
                .Hidden();

            HoverPanelRightTooltip = new RichParagraph();

            HoverPanelRight.AddChild(HoverPanelRightTooltip);

            GameViewPanel.AddChild(HoverPanelRight);

            RootPanel.AddChild(GameViewPanel);
        }

        protected void CreateStatusPanel()
        {
            StatusParagraph = new RichParagraph()
                .Anchor(Anchor.BottomCenter)
                .NoPadding()
                .Height(0.1f);

            StatusParagraph.BackgroundColor = Color.Black;

            BottomPanel.AddChild(StatusParagraph);
        }
     
        protected void CreateMessageLog()
        {
            _messageLog = new SelectList()
                .SkinSimple()
                .Anchor(Anchor.Auto)
                .NoPadding();

            _messageLog.ExtraSpaceBetweenLines = -14;
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

        protected virtual void ResetViews()
        {
            _gameCamera.Reset();
            _messageLog.ClearItems();
            PlayerPanel.Reset();
        }

        protected override void ViewModelChanged()
        {
            base.ViewModelChanged();
            StatusParagraph.Text = String.Empty;

            UpdateMonsterStatus();
            UpdateMessageLog();
            UpdatePlayerStatus();
        }

        private void UpdateMessageLog()
        {
            var newMessages = _viewModel.MessageStatus.GetUnprocessedMessages();

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

            var newMonsterStatuses = _viewModel.MonsterStatusInView
                .OrderBy(m => m.DistanceFromPlayer)
                .ToList();

            var mergedMonsterPanelStatusQuery = (
                from monsterStatus in newMonsterStatuses
                join panel in MonsterPanels on monsterStatus.ID equals panel.ActorStatus.ID into gj
                from subPanel in gj.DefaultIfEmpty()
                select new
                {
                    MonsterStatus = monsterStatus,
                    MonsterPanel = subPanel ?? new MonsterPanel(monsterStatus)
                }).ToList();

            MonsterPanels.Clear();

            foreach (var monsterJoin in mergedMonsterPanelStatusQuery)
            {
                monsterJoin.MonsterPanel.Update(monsterJoin.MonsterStatus);

                MonsterPanels.Add(monsterJoin.MonsterPanel);
                monsterJoin.MonsterPanel.AddAsChildTo(LeftPanel);
            }
        }

        protected string DelimitWithDashes(string text)
        {
            return $"--- {text} ---";
        }

        public virtual Task Handle(MouseHoverViewNotification notification, CancellationToken cancellationToken)
        {
            if (!IsVisible)
                return Unit.Task;

            var ray = _gameCamera.GetPointerRay(notification.X, notification.Y);

            var gameObjectInformation = _viewModel.GetGameObjectInformationAt(ray);

            if (!String.IsNullOrEmpty(gameObjectInformation))
            {
                var direction = _viewModel.GetMapQuadrantOfRay(ray);

                if (direction == Direction.Right || direction == Direction.DownRight || direction == Direction.UpRight)
                {
                    HoverPanelLeftTooltip.Text = gameObjectInformation;
                    HoverPanelLeft.Visible = true;
                    HoverPanelRight.Visible = false;
                }
                else
                {
                    HoverPanelRightTooltip.Text = gameObjectInformation;
                    HoverPanelLeft.Visible = false;
                    HoverPanelRight.Visible = true;
                }
            }
            else
            {
                HoverPanelLeft.Visible = false;
                HoverPanelRight.Visible = false;
            }

            return Unit.Task;
        }
    }
}