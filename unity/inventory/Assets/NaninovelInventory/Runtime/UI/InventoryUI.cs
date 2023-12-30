using System;
using System.Collections.Generic;
using System.Linq;
using Naninovel;
using Naninovel.UI;
using UnityEngine;

namespace NaninovelInventory
{
    /// <summary>
    /// Component applied to the root of inventory UI prefab.
    /// </summary>
    public class InventoryUI : CustomUI
    {
        /// <summary>
        /// Represents serializable inventory state.
        /// More info about using custom state in Naninovel: https://naninovel.com/guide/state-management.html#custom-state
        /// </summary>
        [Serializable]
        private new class GameState
        {
            public Vector3 Position;
            public InventorySlot.State[] Slots;
        }

        [Tooltip("Number of slots the inventory has.")]
        [SerializeField] private int capacity = 80;
        [Tooltip("Reference to the inventory grid layout component.")]
        [SerializeField] private InventoryGrid grid;
        [Tooltip("Content of the inventory.")]
        [SerializeField] private RectTransform content;

        private InventoryManager inventoryManager;
        private IInputManager inputManager;

        /// <summary>
        /// Returns number of items with the provided ID currently assigned
        /// to the inventory slots.
        /// </summary>
        /// <param name="itemId">Identifier of the item to count.</param>
        public int CountItem (string itemId) => grid.InventorySlots.Sum(s => !s.Empty && s.Item.Id == itemId ? s.StackCount : 0);

        /// <summary>
        /// Attempts to add an item with the provided <paramref name="itemId"/> to the first 
        /// empty inventory slot, or slot with the same item, when stack limit allows.
        /// </summary>
        /// <param name="itemId">Identifier of the item to add.</param>
        /// <param name="amount">Number of items to add.</param>
        /// <returns>Whether the item(s) were added.</returns>
        public async UniTask<bool> AddItemAsync (string itemId, int amount = 1)
        {
            // Check if we can stack the items.
            var stackSlotIndex = grid.InventorySlots.IndexOf(CanStack);
            if (stackSlotIndex >= 0) return await AddItemAtAsync(itemId, stackSlotIndex, amount);

            // In case can't stack, find first empty slot.
            var emptySlotIndex = grid.InventorySlots.IndexOf(s => s.Empty);
            if (emptySlotIndex < 0)
            {
                Debug.LogError($"Failed to add `{itemId}`: no empty slots available.");
                return false;
            }

            return await AddItemAtAsync(itemId, emptySlotIndex, amount);

            bool CanStack (InventorySlot slot)
            {
                if (slot.Empty || slot.Item.Id != itemId) return false;
                return slot.Item.StackCountLimit >= slot.StackCount + amount;
            }
        }

        /// <summary>
        /// Attempts to add an item with the provided <paramref name="itemId"/> to an inventory 
        /// slot with the provided <paramref name="slotIndex"/>.
        /// </summary>
        /// <param name="itemId">Identifier of the item to add.</param>
        /// <param name="slotIndex">Index of the inventory slot to put the item into.</param>
        /// <param name="amount">Number of items to add.</param>
        /// <returns>Whether the item(s) were added.</returns>
        public async UniTask<bool> AddItemAtAsync (string itemId, int slotIndex, int amount = 1)
        {
            if (!grid.InventorySlots.IsIndexValid(slotIndex))
            {
                Debug.LogError($"Failed to add `{itemId}` to `{slotIndex}` slot: slot with the provided ID doesn't exist.");
                return false;
            }
            var slot = grid.InventorySlots[slotIndex];

            var item = await inventoryManager.GetItemAsync(itemId);
            if (!ObjectUtils.IsValid(item))
            {
                Debug.LogError($"Failed to add `{itemId}` to `{slot}` slot: item with the provided ID doesn't exist.");
                return false;
            }

            if (!slot.Empty && slot.Item.Id != itemId)
            {
                Debug.LogError($"Failed to add `{itemId}` to `{slot}` slot: slot is already occupied with `{slot.Item.Id}` item.");
                return false;
            }

            if (!slot.Empty && slot.StackCount + amount > item.StackCountLimit)
            {
                Debug.LogError($"Failed to add `{itemId}` (x{amount}) to `{slot}` slot: exceeding stack count limit.");
                return false;
            }

            if (slot.Empty) slot.SetItem(item);
            slot.SetStackCount(slot.StackCount + amount);

            return true;
        }

        /// <summary>
        /// Attempts to remove item with the provided <paramref name="itemId"/> from the inventory.
        /// </summary>
        /// <param name="itemId">Identifier of the item to remove.</param>
        /// <param name="amount">Number of items to remove.</param>
        /// <returns>Whether the item was removed.</returns>
        public bool RemoveItem (string itemId, int amount = 1)
        {
            while (amount > 0)
            {
                var slot = grid.InventorySlots.FirstOrDefault(s => !s.Empty && s.Item.Id == itemId);
                if (slot is null)
                {
                    Debug.LogError($"Failed to remove `{itemId}` item (x{amount}): item is not added to the inventory or not enough stacks.");
                    return false;
                }

                var removeCount = Mathf.Min(amount, slot.StackCount);
                RemoveItemAt(slot.Id, removeCount);
                amount -= removeCount;
            }
            return true;
        }

        /// <summary>
        /// Attempts to remove an item from an inventory slot with the provided <paramref name="slotIndex"/>.
        /// </summary>
        /// <param name="slotIndex">Index of the inventory slot to remove item from.</param>
        /// <param name="amount">Number of items to remove.</param>
        /// <returns>Whether the item was removed.</returns>
        public bool RemoveItemAt (int slotIndex, int amount = 1)
        {
            if (!grid.InventorySlots.IsIndexValid(slotIndex))
            {
                Debug.LogError($"Failed to remove an item from `{slotIndex}` slot: slot with the provided ID doesn't exist.");
                return false;
            }
            var slot = grid.InventorySlots[slotIndex];

            if (slot.Empty)
            {
                Debug.LogError($"Failed to remove an item from `{slotIndex}` slot: no item is assigned to the slot.");
                return false;
            }

            if (slot.StackCount < amount)
            {
                Debug.LogError($"Failed to remove `{slot.Item.Id}` item (x{amount}) from `{slotIndex}` slot: not enough stacks.");
                return false;
            }

            slot.SetStackCount(slot.StackCount - amount);
            if (slot.StackCount == 0) slot.SetItem(null);
            return true;
        }

        /// <summary>
        /// Removes all the items assigned to inventory slots.
        /// </summary>
        public void RemoveAllItems ()
        {
            foreach (var slot in grid.InventorySlots)
                slot.SetEmpty();
        }

        /// <summary>
        /// Attempts to use an item with the provided <paramref name="itemId"/>.
        /// </summary>
        /// <param name="itemId">Identifier of the item to use.</param>
        /// <returns>Whether the item was found and used.</returns>
        public bool UseItem (string itemId)
        {
            var slot = grid.InventorySlots.FirstOrDefault(s => !s.Empty && s.Item.Id == itemId);
            if (slot is null)
            {
                Debug.LogError($"Failed to use `{itemId}` item: item doesn't exist in inventory.");
                return false;
            }

            slot.Item.Use(slot.Id);

            return true;
        }

        /// <summary>
        /// Attempts to use an item with the provided <paramref name="slotIndex"/>.
        /// </summary>
        public virtual void UseItemAtSlot (int slotIndex)
        {
            if (!grid.InventorySlots.IsIndexValid(slotIndex)) return;

            var slot = grid.InventorySlots[slotIndex];
            if (slot.Empty) return;

            slot.Item.Use(slotIndex);
        }

        protected override void Awake ()
        {
            base.Awake();
            this.AssertRequiredObjects(grid, content); // make sure the required objects are assigned in the inspector

            // Store reference to the engine services -- we'll need them later.
            inventoryManager = Engine.GetService<InventoryManager>();
            inputManager = Engine.GetService<IInputManager>();

            grid.Initialize(capacity);
            grid.OnSlotClicked += UseItemAtSlot;
        }

        protected override void OnEnable ()
        {
            base.OnEnable();

            // Start listening for `ToggleInventory` input event to toggle UI's visibility.
            var toggleSampler = inputManager.GetSampler("ToggleInventory");
            if (toggleSampler != null)
                toggleSampler.OnStart += ToggleVisibility;
        }

        protected override void OnDisable ()
        {
            base.OnDisable();

            // Stop listening for `ToggleInventory` input event.
            var toggleSampler = inputManager?.GetSampler("ToggleInventory");
            if (toggleSampler != null)
                toggleSampler.OnStart -= ToggleVisibility;
        }

        protected override void SerializeState (GameStateMap stateMap)
        {
            // Invoked when the game is saved.

            base.SerializeState(stateMap);

            // Serialize UI state.
            var state = new GameState {
                Position = content.transform.position,
                Slots = grid.InventorySlots.Select(s => s.GetSate()).ToArray()
            };
            stateMap.SetState(state);
        }

        protected override async UniTask DeserializeState (GameStateMap stateMap)
        {
            // Invoked when the game is loaded.

            await base.DeserializeState(stateMap);

            RemoveAllItems();

            var state = stateMap.GetState<GameState>();
            if (state is null) return; // empty state, do nothing

            // Restore UI state.
            if (state.Slots?.Length > 0)
            {
                var tasks = new List<UniTask>();
                for (int i = 0; i < state.Slots.Length; i++)
                    if (!string.IsNullOrEmpty(state.Slots[i].ItemId))
                        tasks.Add(AddItemAtAsync(state.Slots[i].ItemId, i, state.Slots[i].StackCount));
                await UniTask.WhenAll(tasks);
            }
            content.transform.position = state.Position;
        }
    }
}
