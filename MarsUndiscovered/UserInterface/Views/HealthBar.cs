using System;

using FrigidRogue.MonoGame.Core.View.Extensions;

using GeonBit.UI.Entities;

using Microsoft.Xna.Framework;

namespace MarsUndiscovered.UserInterface.Views
{
    public class HealthBar : BaseCompositeEntity
    {
        private ProgressBar _healthBar;
        private Label _healthLabel;
        private Label _gainLossLabel;

        public HealthBar()
        {
            _healthBar = new ProgressBar()
                .NoPadding()
                .Anchor(Anchor.Auto)
                .TransparentFillColor()
                .ProgressBarFillColor(Color.Red)
                .Locked();

            _healthBar.Value = 0;

            _healthLabel = new Label("Health")
                .NoPadding()
                .Anchor(Anchor.Center);

            _healthBar.AddChild(_healthLabel);

            _gainLossLabel = new Label("")
                .NoPadding()
                .Anchor(Anchor.CenterRight);

            _healthBar.AddChild(_gainLossLabel);
        }

        public override void AddAsChildTo(Entity parent)
        {
            parent.AddChild(_healthBar);
        }

        public override void RemoveFromParent()
        {
            _healthBar.RemoveFromParent();
        }

        public void UpdateHealth(int currentHealth, int maxHealth)
        {
            var oldHealth = _healthBar.Value;

            _healthBar.Max = (uint)maxHealth;
            _healthBar.StepsCount = _healthBar.Max;
            _healthBar.Value = currentHealth;

            _gainLossLabel.Text = String.Empty;

            if (currentHealth != oldHealth && oldHealth != 0)
            {
                var percentage = 0;
                string sign = String.Empty;

                if (oldHealth > 0)
                {
                    percentage = 100 - ((currentHealth * 100) / oldHealth);

                    if (currentHealth < oldHealth)
                        percentage = -percentage;
                }

                if (percentage != 0)
                    _gainLossLabel.Text = $"({percentage}%)";
            }
        }
    }
}