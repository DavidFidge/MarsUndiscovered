namespace MarsUndiscovered.Game.Components
{
    public class ItemGroup : List<Item>
    {
        public ItemGroup(Item item)
        {
            Add(item);
        }

        public ItemGroup(List<Item> item)
        {
            AddRange(item);
        }
    }
}
