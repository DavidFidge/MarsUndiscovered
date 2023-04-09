using MarsUndiscovered.Game.Components.Dto;

namespace MarsUndiscovered.UserInterface.Views
{
    public class MonsterPanel : ActorPanel
    {
        public MonsterPanel(MonsterStatus monsterStatus)
        {
            Update(monsterStatus);
        }
    }
}