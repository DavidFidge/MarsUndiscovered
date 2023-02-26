namespace MarsUndiscovered.Components
{
    public abstract class VictoryItemType : ItemType
    {
        public override string GetLongDescription(ItemTypeDiscovery itemTypeDiscovery)
        {
            return "An item that must be obtained to win the game.";
        }
    }
}