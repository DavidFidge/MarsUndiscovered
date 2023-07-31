using FrigidRogue.MonoGame.Core.View.Extensions;
using GeonBit.UI.Entities;
using MarsUndiscovered.Game.Components.Dto;

namespace MarsUndiscovered.UserInterface.Views
{
    public class MonsterPanel : ActorPanel<MonsterStatus>
    {
        public Label Status { get; set; }
        
        public MonsterPanel(MonsterStatus monsterStatus)
        {
            Status = new Label()
                .NoPadding()
                .WidthOfContainer()
                .Anchor(Anchor.Auto);
    
            Panel.AddChild(Status);
            
            Update(monsterStatus);
        }

        public override void Update(MonsterStatus actorStatus)
        {
            base.Update(actorStatus);
            Status.Text = actorStatus.Behaviour ?? String.Empty;
        }
    }
}