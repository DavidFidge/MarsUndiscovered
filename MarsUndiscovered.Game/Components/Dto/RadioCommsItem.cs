using GoRogue.GameFramework;

namespace MarsUndiscovered.Game.Components.Dto
{
    public class RadioCommsItem
    {
        public string Message { get; }
        public string Source { get; }
        public IGameObject GameObject { get; }

        public RadioCommsItem(RadioCommsEntry radioCommsEntry)
        {
            GameObject = radioCommsEntry.GameObject;
            Message = radioCommsEntry.Message;
            Source = radioCommsEntry.Source;
        }
    }
}