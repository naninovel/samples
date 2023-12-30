using UnityEngine;
using UnityEngine.Events;

namespace NaninovelInventory
{
    /// <summary>
    /// Component attached to the root of inventory item prefab.
    /// </summary>
    public class InventoryItem : MonoBehaviour
    {
        [System.Serializable]
        public class ItemUsedEvent : UnityEvent<int> { }

        public string Id => name;
        public int StackCountLimit => stackCountLimit;

        [Tooltip("How many items of this type can be stacked on a single inventory slot.")]
        [SerializeField] private int stackCountLimit = 1;
        [Tooltip("Invoked when the item is used; returns slot index of the used item.")]
        [SerializeField] private ItemUsedEvent onItemUsed = default;

        /// <summary>
        /// Use the item. Override the method in inherited types 
        /// or use <see cref="onItemUsed"/> event for item-specific effects.
        /// </summary>
        /// <param name="slotIndex">Index of the slot the item was used from.</param>
        public virtual void Use (int slotIndex)
        {
            onItemUsed?.Invoke(slotIndex);
        }
    }
}
