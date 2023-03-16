using GoRogue.Random;

namespace MarsUndiscovered.Components;

public class ItemTypeDiscoveryCollection : Dictionary<ItemType, ItemTypeDiscovery>
{
    public ItemTypeDiscoveryCollection()
    {}
    
    public ItemTypeDiscoveryCollection(Dictionary<string, ItemTypeDiscovery> itemTypeDiscoveries)
    {
        foreach (var itemTypeDiscovery in itemTypeDiscoveries)
        {
            Add(ItemType.ItemTypes[itemTypeDiscovery.Key], itemTypeDiscovery.Value);
        }
    }
    
    private static string[] RandomFlaskNames =
    {
        "A Red",
        "An Orange",
        "A Yellow",
        "A Blue",
        "A Green",
        "A Purple",
        "A White",
        "A Black",
        "A Violet",
        "A Magenta",
        "An Aqua",
        "A Turquoise"
    };

    private static string[] RandomGadgetNames =
    {
        "A Shiny",
        "A Sparkling",
        "A Smooth",
        "A Rough",
        "A Vibrating",
        "A Warm",
        "A Cold",
        "A Dull",
        "A Mysterious",
        "A Complicated",
        "An Intricate",
        "A Fiddly"
    };

    public bool IsItemTypeDiscovered(ItemType itemType)
    {
        return this[itemType].IsItemTypeDiscovered;
    }
    
    public bool IsItemTypeDiscovered(Item item)
    {
        return this[item.ItemType].IsItemTypeDiscovered;
    }
    
    public void SetItemTypeDiscovered(Item item)
    {
        this[item.ItemType].IsItemTypeDiscovered = true;
    }
    
    public void SetItemTypeUndiscovered(Item item)
    {
        this[item.ItemType].IsItemTypeDiscovered = false;
    }
    
    public void SetItemTypeDiscovered(ItemType itemType)
    {
        this[itemType].IsItemTypeDiscovered = true;
    }
    
    public void SetItemTypeUndiscovered(ItemType itemType)
    {
        this[itemType].IsItemTypeDiscovered = false;
    }

    public void CreateUndiscoveredItemTypeNames()
    {
        var unusedFlaskNames = new List<string>(RandomFlaskNames);

        var index = GlobalRandom.DefaultRNG.NextInt(0, unusedFlaskNames.Count - 1);
        Add(ItemType.HealingBots, new ItemTypeDiscovery(unusedFlaskNames[index]));
        unusedFlaskNames.Remove(unusedFlaskNames[index]);

        var unusedGadgetNames = new List<string>(RandomGadgetNames);
        index = GlobalRandom.DefaultRNG.NextInt(0, unusedGadgetNames.Count - 1);
        Add(ItemType.ShieldGenerator, new ItemTypeDiscovery(unusedGadgetNames[index]));
        unusedGadgetNames.Remove(unusedGadgetNames[index]);
    }
    
    public string GetUndiscoveredDescription(ItemType itemType)
    {
        TryGetValue(itemType, out var itemTypeDiscovery);

        if (itemTypeDiscovery == null)
            return null;
        
        return $"{itemTypeDiscovery.UndiscoveredName} {itemType.GetAbstractTypeDescription()}";
    }
    
    public string GetUndiscoveredDescription(Item item)
    {
        return GetUndiscoveredDescription(item.ItemType);
    }
    
    public string GetInventoryDescriptionWithoutPrefix(Item item)
    {
        TryGetValue(item.ItemType, out var itemTypeDiscovery);
            
        return item.GetDescriptionWithoutPrefix(itemTypeDiscovery);
    }

    public string GetInventoryDescriptionAsSingleItem(Item item)
    {
        TryGetValue(item.ItemType, out var itemTypeDiscovery);
            
        return item.GetDescriptionWithoutStatus(itemTypeDiscovery);
    }

    public string GetInventoryDescriptionAsSingleItemLowerCase(Item item)
    {
        var itemDescription = GetInventoryDescriptionAsSingleItem(item);
        return $"{itemDescription.Substring(0, 1).ToLower()}{itemDescription.Substring(1)}";
    }
}