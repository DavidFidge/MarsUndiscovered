using FrigidRogue.MonoGame.Core.View.Extensions;

using GeonBit.UI.Entities;

using MarsUndiscovered.Components;

namespace MarsUndiscovered.UserInterface.Views
{
    public abstract class ActorPanel : BaseCompositeEntity
    {
        protected Panel Panel;
        protected HealthBar HealthBar;
        protected Label Name;

        public ActorPanel()
        {
            Panel = new Panel()
                .NoSkin()
                .NoPadding()
                .WidthOfScreen()
                .AutoHeight()
                .Anchor(Anchor.Auto);

            HealthBar = new HealthBar();

            Name = new Label()
                .NoPadding()
                .WidthOfScreen()
                .Anchor(Anchor.Auto);

            Panel.AddChild(Name);
            HealthBar.AddAsChildTo(Panel);
        }

        public void Update(ActorStatus actorStatus)
        {
            Name.Text = actorStatus.Name;
            HealthBar.UpdateHealth(actorStatus.Health, actorStatus.MaxHealth);
        }

        public override void AddAsChildTo(Entity parent)
        {
            parent.AddChild(Panel);
        }

        public override void RemoveFromParent()
        {
            Panel.RemoveFromParent();
        }
    }
}