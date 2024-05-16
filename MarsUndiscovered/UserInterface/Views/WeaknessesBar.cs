using FrigidRogue.MonoGame.Core.View.Extensions;

using GeonBit.UI.Entities;
using MarsUndiscovered.Game.Components.Dto;
using MarsUndiscovered.Interfaces;

namespace MarsUndiscovered.UserInterface.Views
{
    public class WeaknessesBar : BaseCompositeEntity
    {
        private readonly IAssets _assets;
        private Panel _weaknessesBarContainer;
        private Dictionary<WeaknessesEnum, WeaknessItem> _weaknessItems = new();
        private bool _isInitialized;
        
        public WeaknessesBar(IAssets assets)
        {
            _assets = assets;
            _weaknessesBarContainer = new Panel()
                .NoPadding()
                .SkinNone()
                .AutoHeight()
                .WidthOfContainer()
                .Anchor(Anchor.Auto);
            
            // This is needed otherwise the first Draw call does not draw the panel with the correct height
            _weaknessesBarContainer.SetHeightBasedOnChildren();
        }

        public override void AddAsChildTo(Entity parent)
        {
            parent.AddChild(_weaknessesBarContainer);
        }

        public override void RemoveFromParent()
        {
            _weaknessesBarContainer.RemoveFromParent();
        }

        public void Initialize(ActorStatus actorStatus)
        {
            if (_isInitialized)
                return;

            _isInitialized = true;
            
            if (actorStatus.CanBeConcussed)
            {
                var concussImage = new WeaknessItem(_assets, "Concuss");
                _weaknessItems.Add(WeaknessesEnum.Concuss, concussImage);
                concussImage.AddAsChildTo(_weaknessesBarContainer);

                if (actorStatus.IsConcussed)
                    concussImage.Activate();
            }
        }

        public void Update(ActorStatus actorStatus)
        {
            Initialize(actorStatus);
            
            if (actorStatus.CanBeConcussed)
            {
                _weaknessItems[WeaknessesEnum.Concuss].ActivateDeactivate(actorStatus.IsConcussed); 
            }
        }
    }
}
