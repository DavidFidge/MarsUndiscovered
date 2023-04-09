using FrigidRogue.MonoGame.Core.View.Extensions;

using GeonBit.UI.Entities;
using MarsUndiscovered.Game.Components.Dto;

namespace MarsUndiscovered.UserInterface.Views
{
    public abstract class ActorPanel : BaseCompositeEntity
    {
        protected Panel Panel;
        protected HealthBar HealthBar;
        protected Label Name;
        public ActorStatus ActorStatus { get; private set; }

        public ActorPanel()
        {
            Panel = new Panel()
                .SkinNone()
                .NoPadding()
                .WidthOfContainer()
                .AutoHeight()
                .Anchor(Anchor.Auto);

            HealthBar = new HealthBar();

            Name = new Label()
                .NoPadding()
                .WidthOfContainer()
                .Anchor(Anchor.Auto);

            Panel.AddChild(Name);
            HealthBar.AddAsChildTo(Panel);
        }

        public void Update(ActorStatus actorStatus)
        {
            ActorStatus = actorStatus;
            Name.Text = actorStatus.Name;
            HealthBar.UpdateHealth(actorStatus.Health, actorStatus.MaxHealth, actorStatus.Shield);
        }

        public override void AddAsChildTo(Entity parent)
        {
            parent.AddChild(Panel);
        }

        public override void RemoveFromParent()
        {
            Panel.RemoveFromParent();
        }
        
        public void Reset()
        {
            HealthBar.Reset();
        }
    }
}