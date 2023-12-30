using System;
using Naninovel;

namespace NaninovelInventory
{
    /// <summary>
    /// Represents an inventory slot.
    /// </summary>
    public class InventorySlot
    {
        /// <summary>
        /// Serializable state of the slot.
        /// </summary>
        [Serializable]
        public struct State
        {
            public string ItemId;
            public int StackCount;
        }

        /// <summary>
        /// Invoked when assigned item is changed or removed.
        /// </summary>
        public event Action<InventoryItem> OnItemChanged;
        /// <summary>
        /// Invoked when stack count is changed.
        /// </summary>
        public event Action<int> OnStackCountChanged;

        /// <summary>
        /// Index of the slot (zero-based) inside the inventory grid.
        /// </summary>
        public int Id { get; }
        /// <summary>
        /// Item in the slot (if any).
        /// </summary>
        public InventoryItem Item { get; private set; }
        /// <summary>
        /// Number of items stacked in the inventory slot.
        /// </summary>
        public int StackCount { get; private set; }
        /// <summary>
        /// Whether the slot is empty (has no item assigned).
        /// </summary>
        public bool Empty => !ObjectUtils.IsValid(Item);

        public InventorySlot (int id)
        {
            Id = id;
        }

        /// <summary>
        /// Returns serializable state of the slot.
        /// </summary>
        public State GetSate () => new State {
            ItemId = Empty ? null : Item.Id,
            StackCount = StackCount
        };

        /// <summary>
        /// Removes any assigned items and zeros stack count.
        /// </summary>
        public void SetEmpty ()
        {
            SetItem(null);
            SetStackCount(0);
        }

        /// <summary>
        /// Assigns an item to the slot.
        /// </summary>
        public void SetItem (InventoryItem item)
        {
            Item = item;
            OnItemChanged?.Invoke(item);
        }

        /// <summary>
        /// Changes stack count of the current item.
        /// </summary>
        public void SetStackCount (int stackCount)
        {
            StackCount = stackCount;
            OnStackCountChanged?.Invoke(stackCount);
        }
    }
}
