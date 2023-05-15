namespace MarsUndiscovered.Game.Components
{
    public abstract class VictoryItemType : ItemType
    {
        public override string GetLongDescription(Item item, ItemTypeDiscovery itemTypeDiscovery)
        {
            return "An item that must be obtained to win the game.";
        }
    }
}