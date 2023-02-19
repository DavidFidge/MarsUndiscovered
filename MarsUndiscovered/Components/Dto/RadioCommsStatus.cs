using NGenerics.Extensions;

namespace MarsUndiscovered.Components.Dto
{
    public class RadioCommsStatus
    {
        public IList<RadioCommsItem> RadioCommsItems { get; } = new List<RadioCommsItem>();
        public int SeenItemsCount { get; private set; }
        public int ProcessedItemsCount { get; private set; }

        public void AddRadioCommsItems(IList<RadioCommsItem> radioCommsItems)
        {
            RadioCommsItems.AddRange(radioCommsItems);
            SeenItemsCount += radioCommsItems.Count;
        }

        public IEnumerable<RadioCommsItem> GetUnprocessedRadioComms()
        {
            return RadioCommsItems.Skip(ProcessedItemsCount);
        }

        public void SetSeenAllItems()
        {
            ProcessedItemsCount = SeenItemsCount;
        }
    }
}