using Augmented.Messages;
using Augmented.UserInterface.Data;
using Augmented.UserInterface.ViewModels;

using DavidFidge.MonoGame.Core.UserInterface;
using DavidFidge.Monogame.Core.View;
using DavidFidge.Monogame.Core.View.Extensions;

using GeonBit.UI.Entities;

using Microsoft.Xna.Framework;

namespace Augmented.UserInterface.Views
{
    public class GameSpeedView : BaseView<GameSpeedViewModel, GameSpeedData>
    {
        private Label _speedLabel;
        private Label _timeLabel;
        private Button _pauseButton;

        public GameSpeedView(GameSpeedViewModel gameSpeedViewModel)
            : base(gameSpeedViewModel)
        {
            _viewType = ViewType.Component;
        }

        protected override void InitializeInternal()
        {
            var speedPanelWidth = 275f;
            var speedPanelItemHeight = 35f;
            var speedPanelItemWidth = 50f;
            
            var paddingPanel = new Panel()
                .TopPaddingPanelWithoutBorder();

            RootPanel.AddChild(paddingPanel);

            _timeLabel = new Label(
                    "0:00",
                    Anchor.AutoCenter,
                    new Vector2(speedPanelWidth, speedPanelItemHeight))
                .NoPadding()
                .DefaultFontSize();

            RootPanel.AddChild(_timeLabel);

            var speedButtonPanel = new Panel(
                    new Vector2(speedPanelWidth, speedPanelItemHeight),
                    PanelSkin.None,
                    Anchor.AutoCenter)
                .NoPadding();

            RootPanel.AddChild(speedButtonPanel);

            _pauseButton = new Button(
                    "||",
                    ButtonSkin.Default,
                    Anchor.AutoInline,
                    new Vector2(speedPanelItemWidth, speedPanelItemHeight))
                .NoPadding()
                .AsToggle()
                .WithSmallButtonScale()
                .WithParent(speedButtonPanel)
                .SendOnClick(Mediator, new ChangeGameSpeedRequest().TogglePauseGameRequest());

            new Button(
                    "<",
                    ButtonSkin.Default,
                    Anchor.AutoInline,
                    new Vector2(speedPanelItemWidth, speedPanelItemHeight))
                .NoPadding()
                .WithSmallButtonScale()
                .WithParent(speedButtonPanel)
                .SendOnClick(Mediator, new ChangeGameSpeedRequest().DecreaseSpeedRequest());

            new Button(
                    ">",
                    ButtonSkin.Default,
                    Anchor.AutoInline,
                    new Vector2(speedPanelItemWidth, speedPanelItemHeight))
                .NoPadding()
                .WithSmallButtonScale()
                .WithParent(speedButtonPanel)
                .SendOnClick(Mediator, new ChangeGameSpeedRequest().IncreaseSpeedRequest());

            _speedLabel = new Label(
                    "1x",
                    Anchor.Center)
                .DefaultFontSize()
                .NoPadding()
                .WrappedInPanel(
                    Anchor.AutoInline,
                    new Vector2(125f, speedPanelItemHeight),
                    speedButtonPanel);
        }

        protected override void UpdateView()
        {
            var gameSpeed = (Data.GameSpeedPercent / 100m).ToString("0.##");

            if (Data.IsPaused)
            {
                if (!_pauseButton.Checked)
                    _pauseButton.Checked = true;

                _speedLabel.Text = "Paused";
            }
            else
            {
                if (_pauseButton.Checked)
                    _pauseButton.Checked = false;

                _speedLabel.Text = $"{gameSpeed}x";
            }

            _timeLabel.Text = Data.TotalGameTime.ToString(@"hh\:mm\:ss");
        }
    }
}