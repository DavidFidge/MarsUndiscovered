using System.Text;

namespace MarsUndiscovered.Game.Components
{
    public class EnhancementBots : NanoFlask
    {
        public override string Name => nameof(EnhancementBots);

        public override string GetDescription(Item item, ItemDiscovery itemDiscovery,
            ItemTypeDiscovery itemTypeDiscovery, int quantity, bool includePrefix = true, bool includeStatus = true)
        {
            if (!itemTypeDiscovery.IsItemTypeDiscovered)
                return base.GetDescription(item, itemDiscovery, itemTypeDiscovery, quantity, includePrefix);

            if (quantity > 1)
                return $"{(includePrefix ? $"{quantity.ToString()} " : "")}{GetAbstractTypeName()}{(includePrefix ? "s" : "")} of Enhancement Bots";

            return $"{(includePrefix ? "A " : "")}{GetTypeDescription()}";
        }

        public override string GetTypeDescription()
        {
            return $"{GetAbstractTypeName()} of Enhancement Bots";
        }

        public override string GetLongDescription(Item item, ItemTypeDiscovery itemTypeDiscovery)
        {
            var baseLongDescription = base.GetLongDescription(item, itemTypeDiscovery);

            if (!String.IsNullOrEmpty(baseLongDescription))
                return baseLongDescription;
            
            var stringBuilder = new StringBuilder();
            
            stringBuilder.AppendLine(
                $"The swirling blue steamy liquid inside this flask are filled with enhancement nanobots. The nanobots will enhance the object using the intent of the user and the will of the nanobots.");
            
            stringBuilder.AppendLine();

            stringBuilder.Append(
                $"The enhancement level of an item is increased by {{{{L_BLUE}}}}1{{{{DEFAULT}}}}.");

            return stringBuilder.ToString();
        }
    }
}
