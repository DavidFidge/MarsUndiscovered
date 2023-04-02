using FrigidRogue.MonoGame.Core.View.Extensions;

using GeonBit.UI.Entities;

using Microsoft.Xna.Framework;

namespace MarsUndiscovered.UserInterface.Views
{
    public class HealthBar : BaseCompositeEntity
    {
        private Panel _healthBarContainer;
        private ProgressBar _healthBar;
        private ProgressBar _shieldBar;
        private Label _healthLabel;
        private Label _gainLossLabel;
        private bool _firstHealthUpdate = true;

        public HealthBar()
        {
            _healthBarContainer = new Panel()
                .NoPadding()
                .SkinNone()
                .AutoHeight()
                .WidthOfContainer()
                .Anchor(Anchor.Auto);

            _healthBar = new ProgressBar()
                .NoPadding()
                .Anchor(Anchor.TopLeft)
                .TransparentFillColor()
                .ProgressBarFillColor(Color.Red)
                .WidthOfContainer()
                .Locked();

            _shieldBar = new ProgressBar()
                .NoPadding()
                .Anchor(Anchor.TopLeft)
                .TransparentFillColor()
                .ProgressBarFillColor(Color.LightBlue.WithTransparencyPremultiplied(0.9f))
                .WidthOfContainer()
                .Locked();

            _healthBarContainer.AddChild(_healthBar);
            _healthBarContainer.AddChild(_shieldBar);

            _healthBar.Value = 0;

            _healthLabel = new Label("Health")
                .NoPadding()
                .Anchor(Anchor.Center);

            // Add health labels to shield bar instead otherwise the text gets obscured by the shield bar colour
            _shieldBar.AddChild(_healthLabel);

            _gainLossLabel = new Label("")
                .NoPadding()
                .Anchor(Anchor.CenterRight);

            _shieldBar.AddChild(_gainLossLabel);

            // This is needed otherwise the first Draw call does not draw the panel with the correct height
            _healthBarContainer.SetHeightBasedOnChildren();
        }

        public override void AddAsChildTo(Entity parent)
        {
            parent.AddChild(_healthBarContainer);
        }

        public override void RemoveFromParent()
        {
            _healthBarContainer.RemoveFromParent();
        }

        public void UpdateHealth(int currentHealth, int maxHealth, int currentShield)
        {
            var oldHealth = _healthBar.Value;

            if (_firstHealthUpdate)
            {
                oldHealth = currentHealth;
                _firstHealthUpdate = false;
            }
            
            _healthBar.Max = (uint)maxHealth;
            _healthBar.StepsCount = _healthBar.Max;
            _healthBar.Value = currentHealth;

            _gainLossLabel.Text = String.Empty;

            // Percentage display currently does not work for monsters - monsters always have their panels newly created so old health is always zero.
            if (oldHealth != 0)
            {
                var healthDifference = currentHealth - oldHealth;

                if (healthDifference != 0 && maxHealth != 0)
                {
                    var percentage = (healthDifference * 100) / maxHealth;

                    if (percentage != 0)
                        _gainLossLabel.Text = $"({percentage}%)";
                }
            }

            if (currentHealth <= 0)
                _healthLabel.Text = "DEAD";
            else
                _healthLabel.Text = "Health";

            _shieldBar.StepsCount = _healthBar.Max;
            _shieldBar.Max = (uint)maxHealth;
            _shieldBar.Value = currentShield;
        }

        public void Reset()
        {
            _firstHealthUpdate = true;
        }
    }
}
