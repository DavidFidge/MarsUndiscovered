using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using FrigidRogue.MonoGame.Core.Interfaces.Components;
using FrigidRogue.MonoGame.Core.Interfaces.Services;
using FrigidRogue.MonoGame.Core.Services;

using MarsUndiscovered.Components.SaveData;
using MarsUndiscovered.Interfaces;

using Microsoft.Xna.Framework.Input;

using NGenerics.Extensions;

namespace MarsUndiscovered.Components
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

    public class Inventory : IMementoState<InventorySaveData>, ISaveable, ICollection<Item>
    {
        private readonly IGameWorld _gameWorld;

        public List<Item> Items { get; set; }
        public Dictionary<Keys, ItemGroup> ItemKeyAssignments { get; set; } = new Dictionary<Keys, ItemGroup>();
        public Dictionary<Item, string> CallItem { get; set; } = new Dictionary<Item, string>();
        public Dictionary<ItemType, string> CallItemType { get; set; } = new Dictionary<ItemType, string>();

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

        public Keys GetNextUnusedKey()
        {
            return CandidateKeys.First(k => !ItemKeyAssignments.ContainsKey(k));
        }

        public void AssignNewKey(Item item, Keys key)
        {
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

        public Inventory(IGameWorld gameWorld)
        {
            _gameWorld = gameWorld;
        }

        public IMemento<InventorySaveData> GetSaveState(IMapper mapper)
        {
            var mementoForDerivedType = Memento<InventorySaveData>.CreateWithAutoMapper(this, mapper);

            return new Memento<InventorySaveData>(mementoForDerivedType.State);
        }

        public void SetLoadState(IMemento<InventorySaveData> memento, IMapper mapper)
        {
            Memento<InventorySaveData>.SetWithAutoMapper(this, memento, mapper);

            Items = memento.State.ItemIds.Select(s => _gameWorld.Items[s]).ToList();

            ItemKeyAssignments = memento.State.ItemKeyAssignments
                .ToDictionary(k => k.Key, v => new ItemGroup(v.Value.Select(i => _gameWorld.Items[i]).ToList()));

            CallItem = memento.State.CallItem
                .ToDictionary(k => _gameWorld.Items[k.Key], v => v.Value);

            CallItemType = memento.State.CallItemType
                .ToDictionary(k => ItemType.ItemTypes[k.Key], v => v.Value);
        }

        public void SaveState(ISaveGameService saveGameService)
        {
            saveGameService.SaveToStore(GetSaveState(saveGameService.Mapper));
        }

        public void LoadState(ISaveGameService saveGameService)
        {
            var playerSaveData = saveGameService.GetFromStore<InventorySaveData>();
            SetLoadState(playerSaveData, saveGameService.Mapper);
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
            if (item == null)
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

            ItemKeyAssignments.Add(GetNextUnusedKey(), new ItemGroup(item));
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

            Items.Remove(item);

            var key = ItemKeyAssignments.First(v => v.Value.Contains(item)).Key;

            ItemKeyAssignments[key].Remove(item);

            if (ItemKeyAssignments[key].IsEmpty())
            {
                // Only remove from ItemKeyAssignments. CallItem and CallItemType should remain as they are forever
                // in case the user drops items and picks them up again.
                ItemKeyAssignments.Remove(key);
            }

            return true;
        }

        public int Count => ItemKeyAssignments.Count;
        public bool IsReadOnly => false;
    }
}
