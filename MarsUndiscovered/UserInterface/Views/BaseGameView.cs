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
using MonoGame.Extended.Sprites;
using SadRogue.Primitives;

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
        protected Panel RadioCommsPanel;
        protected RichParagraph RadioCommsMessage;
        protected RichParagraph RadioCommsSource;
        protected Image RadioCommsImage;
        protected AnimatedSprite RadioCommsAnimatedSprite;
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
                .Width(0.19f)
                .NoSkin()
                .NoPadding()
                .Height(0.999f);

            RootPanel.AddChild(LeftPanel);

            TopPanel = new Panel()
                .Anchor(Anchor.TopRight)
                .Width(Constants.MiddlePanelWidth)
                .NoSkin()
                .NoPadding()
                .Offset(new Vector2(20f, 0))
                .Height(0.14f);

            RootPanel.AddChild(TopPanel);

            BottomPanel = new Panel()
                .Anchor(Anchor.BottomRight)
                .Width(Constants.MiddlePanelWidth)
                .NoSkin()
                .NoPadding()
                .Offset(new Vector2(20f, 0))
                .Height(0.2f);

            RootPanel.AddChild(BottomPanel);

            GameViewPanel = new Panel()
                .Anchor(Anchor.CenterRight)
                .Width(Constants.MiddlePanelWidth)
                .NoSkin()
                .NoPadding()
                .Height(0.69f)
                .Offset(new Vector2(20f, 120f));

            HoverPanelLeft = new Panel()
                .Anchor(Anchor.TopLeft)
                .Width(0.45f)
                .Skin(PanelSkin.Simple)
                .AutoHeight()
                .Hidden();

            GameViewPanel.AddChild(HoverPanelLeft);

            HoverPanelLeftTooltip = new RichParagraph();

            HoverPanelLeft.AddChild(HoverPanelLeftTooltip);

            HoverPanelRight = new Panel()
                .Anchor(Anchor.TopRight)
                .Width(0.45f)
                .Skin(PanelSkin.Simple)
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
                .Anchor(Anchor.BottomLeft)
                .NoPadding()
                .Height(0.1f);

            BottomPanel.AddChild(StatusParagraph);
        }
        
        protected void CreateRadioCommsPanel()
        {
            RadioCommsPanel = new Panel()
                .Anchor(Anchor.BottomCenter)
                .Skin(PanelSkin.Simple)
                .Height(380)
                .WidthOfContainer();

            BottomPanel.AddChild(RadioCommsPanel);

            RadioCommsSource = new RichParagraph()
                .Anchor(Anchor.AutoCenter)
                .NoPadding();
            
            RadioCommsPanel.AddChild(RadioCommsSource);

            RadioCommsImage = new Image()
                .Anchor(Anchor.AutoInline)
                .Width(256)
                .Height(256)
                .NoPadding();
            
            RadioCommsPanel.AddChild(RadioCommsImage);

            var spacer = new Panel()
                .Anchor(Anchor.AutoInline)
                .NoPadding()
                .NoSkin()
                .Width(0.01f);
            
            RadioCommsPanel.AddChild(spacer);
            
            RadioCommsMessage = new RichParagraph()
                .Anchor(Anchor.AutoInlineNoBreak)
                .Width(0.87f)
                .NoPadding();
            
            RadioCommsPanel.AddChild(RadioCommsMessage);
        }

        protected void CreateMessageLog()
        {
            _messageLog = new SelectList()
                .NoSkin()
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
        }

        protected override void ViewModelChanged()
        {
            base.ViewModelChanged();
            UpdateMonsterStatus();
            UpdateMessageLog();
            UpdatePlayerStatus();
            UpdateRadioComms();
            UpdateMapRenderTargetSize(_viewModel.MapViewModel.Width, _viewModel.MapViewModel.Height);
            StatusParagraph.Text = String.Empty;
        }

        private void UpdateRadioComms()
        {
            var newRadioComms = _viewModel.RadioCommsStatus.GetUnprocessedRadioComms();

            if (newRadioComms.Any())
            {
                // TODO - implement "next" functionality around here so that the user can see multiple radio logs that occur on the same turn
                var lastRadioComms = newRadioComms.Last();

                StatusParagraph.Text = DelimitWithDashes("PRESS SPACE TO CONTINUE");

                RadioCommsMessage.Text = lastRadioComms.Message;
                RadioCommsSource.Text = lastRadioComms.Source;
                
                var radioCommsSpriteSheet = Assets.GetRadioCommsSpriteSheet(lastRadioComms.GameObject);
                
                RadioCommsAnimatedSprite = new AnimatedSprite(radioCommsSpriteSheet);
                RadioCommsAnimatedSprite.Play("talk");
                RadioCommsImage.Texture = RadioCommsAnimatedSprite.TextureRegion.Texture;
                RadioCommsImage.SourceRectangle = RadioCommsAnimatedSprite.TextureRegion.Bounds;
                _viewModel.RadioCommsStatus.SetSeenAllItems();
            }
        }

        private void UpdateMessageLog()
        {
            var newMessages = _viewModel.MessageStatus.GetUnprocessedMessages();

            if (newMessages.Any())
            {
                foreach (var message in newMessages)
                    _messageLog.AddItem(message);

                _viewModel.MessageStatus.SetSeenAllMessages();

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

        public override void Update()
        {
            RadioCommsAnimatedSprite.Update(_viewModel.GameTimeService.GameTime);
            RadioCommsImage.SourceRectangle = RadioCommsAnimatedSprite.TextureRegion.Bounds;
            base.Update();
        }
    }
}