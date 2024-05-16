using FrigidRogue.MonoGame.Core.View.Extensions;

using GeonBit.UI.Entities;
using MarsUndiscovered.Game.Components.Dto;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.UserInterface.Views
{
    public abstract class ActorPanel<T> : BaseCompositeEntity where T : ActorStatus
    {
        protected Panel Panel;
        protected HealthBar HealthBar;
        protected Label Name;
        protected WeaknessesBar WeaknessesBar;
        public T ActorStatus { get; private set; }

        public ActorPanel(IAssets assets)
        {
            CreatePanels(assets);
        }

        public void CreatePanels(IAssets assets)
        {
            Panel = new Panel()
                .SkinNone()
                .NoPadding()
                .WidthOfContainer()
                .AutoHeight()
                .Anchor(Anchor.Auto);

            HealthBar = new HealthBar();
            WeaknessesBar = new WeaknessesBar(assets);

            Name = new Label()
                .NoPadding()
                .WidthOfContainer()
                .Anchor(Anchor.Auto);
            
            Panel.AddChild(Name);
            HealthBar.AddAsChildTo(Panel);
            WeaknessesBar.AddAsChildTo(Panel);
        }

        public virtual void Update(T actorStatus)
        {
            ActorStatus = actorStatus;
            Name.Text = actorStatus.Name;
            HealthBar.UpdateHealth(actorStatus.Health, actorStatus.MaxHealth, actorStatus.Shield);
            WeaknessesBar.Update(actorStatus);
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