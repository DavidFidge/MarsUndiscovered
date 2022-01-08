using MarsUndiscovered.Components;

namespace MarsUndiscovered.UserInterface.Views
{
    public class MonsterPanel : ActorPanel
    {
        public MonsterPanel(MonsterStatus monsterStatus) : base()
        {
            Update(monsterStatus);
        }
    }
}