using FrigidRogue.MonoGame.Core.View.Extensions;

using GeonBit.UI.Entities;
using MarsUndiscovered.Game.Components.Dto;

namespace MarsUndiscovered.UserInterface.Views
{
    public abstract class ActorPanel<T> : BaseCompositeEntity where T : ActorStatus
    {
        protected Panel Panel;
        protected HealthBar HealthBar;
        protected Label Name;
        public T ActorStatus { get; private set; }

        public ActorPanel()
        {
            CreatePanels();
        }

        public void CreatePanels()
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

        public virtual void Update(T actorStatus)
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