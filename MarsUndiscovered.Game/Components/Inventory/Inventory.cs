﻿using System.Collections;

using FrigidRogue.MonoGame.Core.Extensions;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Services;

using MarsUndiscovered.Game.Components.SaveData;
using MarsUndiscovered.Interfaces;

using Microsoft.Xna.Framework.Input;

namespace MarsUndiscovered.Game.Components
{
    public class Inventory : ISaveable, ICollection<Item>
    {
        private readonly IGameWorld _gameWorld;

        private static int ItemLimit => 26;
        public bool CanPickUpItem => Items.Count < ItemLimit;
        public int Count => ItemKeyAssignments.Count;
        public bool IsReadOnly => false;
        public bool HasShipRepairParts => Items.Any(i => i.ItemType is ShipRepairParts);

        public List<Item> Items { get; set; } = new List<Item>();
        public Dictionary<Keys, ItemGroup> ItemKeyAssignments { get; set; } = new Dictionary<Keys, ItemGroup>();
        public Dictionary<Item, string> CallItem { get; set; } = new Dictionary<Item, string>();
        public Dictionary<ItemType, string> CallItemType { get; set; } = new Dictionary<ItemType, string>();
        public ItemTypeDiscoveryCollection ItemTypeDiscoveries { get; set; } = new ItemTypeDiscoveryCollection();
        public Item EquippedWeapon { get; private set; }

        private static readonly Keys[] CandidateKeys = {
            Keys.A,
            Keys.B,
            Keys.C,
            Keys.D,
            Keys.E,
            Keys.F,
            Keys.G,
            Keys.H,
            Keys.I,
            Keys.J,
            Keys.K,
            Keys.L,
            Keys.M,
            Keys.N,
            Keys.O,
            Keys.P,
            Keys.Q,
            Keys.R,
            Keys.S,
            Keys.T,
            Keys.U,
            Keys.V,
            Keys.W,
            Keys.X,
            Keys.Y,
            Keys.Z
        };

        public Inventory(IGameWorld gameWorld)
        {
            _gameWorld = gameWorld;
            ItemTypeDiscoveries.CreateUndiscoveredItemTypeNames();
        }

        public List<InventoryItem> GetInventoryItems()
        {
            var inventoryItems = ItemKeyAssignments.Keys
                .Select(
                    key =>
                    {
                        var item = ItemKeyAssignments[key].First();

                        ItemTypeDiscoveries.TryGetValue(item.ItemType, out var itemTypeDiscovery);

                        return new InventoryItem
                        {
                            ItemDescription = item.GetDescription(
                                itemTypeDiscovery,
                                ItemKeyAssignments[key].Count
                            ),
                            ItemDiscoveredDescription = item.GetDiscoveredDescription(ItemKeyAssignments[key].Count),
                            ItemType = item.ItemType,
                            Key = key,
                            LongDescription = item.GetLongDescription(itemTypeDiscovery),
                            CanEquip = CanEquip(item),
                            CanDrop = true,
                            CanUnequip = CanUnequip(item),
                            CanApply = CanApply(item),
                            CanEnchant = CanEnchant(item)
                        };
                    }
                ).ToList();

            return inventoryItems;
        }

        public Keys GetNextUnusedKey()
        {
            return CandidateKeys.First(k => !ItemKeyAssignments.ContainsKey(k));
        }

        public void AssignNewKey(Item item, Keys key)
        {
            if (!CandidateKeys.Contains(key))
                return;

            var currentItemAssignment = ItemKeyAssignments.First(i => i.Value.Contains(item));

            if (ItemKeyAssignments.ContainsKey(key))
            {
                var existingItem = ItemKeyAssignments[key];
                ItemKeyAssignments[currentItemAssignment.Key] = existingItem;
            }
            else
            {
                ItemKeyAssignments.Remove(currentItemAssignment.Key);
            }

            ItemKeyAssignments[key] = currentItemAssignment.Value;
        }

        public Item GetItemForKey(Keys key)
        {
            if (ItemKeyAssignments.ContainsKey(key))
                return ItemKeyAssignments[key].First();

            return null;
        }

        public void SaveState(ISaveGameService saveGameService, IGameWorld gameWorld)
        {
            var memento = new Memento<InventorySaveData>(new InventorySaveData());
            memento.State.ItemIds = Items.Select(i => i.ID).ToList();
            memento.State.ItemKeyAssignments = ItemKeyAssignments.ToDictionary(k => k.Key, v => v.Value.Select(i => i.ID).ToList());
            memento.State.CallItem = CallItem.ToDictionary(k => k.Key.ID, v => v.Value);
            memento.State.CallItemType = CallItemType.ToDictionary(k => k.Key.Name, v => v.Value);
            memento.State.ItemTypeDiscoveries = ItemTypeDiscoveries.ToDictionary(k => k.Key.Name, v => v.Value);
            memento.State.EquippedWeaponId = EquippedWeapon?.ID;
            
            saveGameService.SaveToStore(memento);
        }

        public void LoadState(ISaveGameService saveGameService, IGameWorld gameWorld)
        {
            var inventorySaveData = saveGameService.GetFromStore<InventorySaveData>();
            
            Items = inventorySaveData.State.ItemIds.Select(s => gameWorld.Items[s]).ToList();

            ItemKeyAssignments = inventorySaveData.State.ItemKeyAssignments
                .ToDictionary(k => k.Key, v => new ItemGroup(v.Value.Select(i => gameWorld.Items[i]).ToList()));

            CallItem = inventorySaveData.State.CallItem
                .ToDictionary(k => gameWorld.Items[k.Key], v => v.Value);

            CallItemType = inventorySaveData.State.CallItemType
                .ToDictionary(k => ItemType.ItemTypes[k.Key], v => v.Value);

            ItemTypeDiscoveries = new ItemTypeDiscoveryCollection(inventorySaveData.State.ItemTypeDiscoveries);

            if (inventorySaveData.State.EquippedWeaponId != null)
                EquippedWeapon = gameWorld.Items[inventorySaveData.State.EquippedWeaponId.Value];
        }

        public IEnumerator<Item> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public void Add(Item item)
        {
            Add(item, Keys.None);
        }

        public void Add(Item item, Keys specificKey)
        {
            if (item == null)
                return;

            if (Items.Contains(item))
                return;

            Items.Add(item);

            if (item.GroupsInInventory)
            {
                var itemGroup = ItemKeyAssignments.Values.FirstOrDefault(ig => ig.First().CanGroupWith(item));
                if (itemGroup != null)
                {
                    itemGroup.Add(item);
                    return;
                }
            }

            if (specificKey != Keys.None && !CandidateKeys.Contains(specificKey))
                throw new Exception($"Specific key {specificKey} must be a-z");

            ItemKeyAssignments.Add(specificKey == Keys.None ? GetNextUnusedKey() : specificKey, new ItemGroup(item));
        }

        public void Clear()
        {
            Items.Clear();
            ItemKeyAssignments.Clear();
        }

        public bool Contains(Item item)
        {
            return Items.Contains(item);
        }

        public void CopyTo(Item[] array, int arrayIndex)
        {
            Items.CopyTo(array, arrayIndex);
        }

        public bool Remove(Item item)
        {
            if (!Items.Contains(item))
                return false;

            Unequip(item);
            Items.Remove(item);

            var key = ItemKeyAssignments.First(v => v.Value.Contains(item)).Key;

            ItemKeyAssignments[key].Remove(item);

            if (ItemKeyAssignments[key].IsEmpty())
            {
                // Only remove from ItemKeyAssignments. CallItem and CallItemType should remain as they are forever
                // in case the user drops items and picks them up again.
                ItemKeyAssignments.Remove(key);
            }
            
            _gameWorld.Player.RecalculateAttacks();

            return true;
        }

        public void Equip(Item item)
        {
            if (!CanEquip(item))
                return;

            if (item.ItemType is Weapon)
            {
                EquippedWeapon = item;
                item.IsEquipped = true;
            }
            
            _gameWorld.Player.RecalculateAttacks();
        }

        public bool CanEquip(Item item)
        {
            if (item == null)
                return false;

            if (!Items.Contains(item))
                return false;

            if (IsEquipped(item))
                return false;

            return CanTypeBeEquipped(item);
        }

        public bool CanTypeBeEquipped(Item item)
        {
            if (item.ItemType is Weapon)
            {
                return true;
            }

            return false;
        }
        
        public bool CanApply(Item item)
        {
            if (item == null)
                return false;

            if (!Items.Contains(item))
                return false;

            return CanTypeBeApplied(item);
        }
        
        public bool CanTypeBeApplied(Item item)
        {
            if (item.ItemType is Gadget or NanoFlask)
            {
                return true;
            }

            return false;
        }

        public bool CanEnchant(Item item)
        {
            if (item == null)
                return false;

            if (!Items.Contains(item))
                return false;

            return CanTypeBeEnchanted(item);
        }
        
        public bool CanTypeBeEnchanted(Item item)
        {
            if (item.ItemType is Gadget or Weapon)
            {
                return true;
            }

            return false;
        }
        
        public bool IsEquipped(Item item)
        {
            if (item == null)
                return false;

            return item.IsEquipped;
        }

        public void Unequip(Item item)
        {
            if (!CanUnequip(item))
                return;

            if (item == EquippedWeapon)
                EquippedWeapon = null;

            item.IsEquipped = false;
            
            _gameWorld.Player.RecalculateAttacks();
        }

        public bool CanUnequip(Item item)
        {
            if (item == null)
                return false;

            if (!Items.Contains(item))
                return false;

            if (item == EquippedWeapon)
                return true;

            return false;
        }

        public Item GetEquippedItemOfType(ItemType itemType)
        {
            if (itemType is Weapon)
                return EquippedWeapon;

            return null;
        }

        public Keys GetKeyForItem(Item item)
        {
            return ItemKeyAssignments.First(i => i.Value.Contains(item)).Key;
        }
        
        public bool IsIdentified(Item item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (!ItemTypeDiscoveries.IsItemTypeDiscovered(item))
                return false;
            
            if (!item.ItemDiscovery.IsEnchantLevelDiscovered)
                return false;

            if (!item.ItemDiscovery.IsItemSpecialDiscovered)
                return false;

            return true;
        }
    }
}
