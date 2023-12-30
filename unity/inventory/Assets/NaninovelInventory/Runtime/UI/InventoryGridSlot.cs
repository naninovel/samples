using Naninovel;
using UnityEngine;

namespace NaninovelInventory
{
    /// <summary>
    /// Attached to the root object of inventory slot UI prefab.
    /// </summary>
    public class InventoryGridSlot : ScriptableGridSlot
    {
        /// <summary>
        /// Current slot model bound to the UI.
        /// </summary>
        public InventorySlot BoundSlot { get; private set; }
        public override string Id => BoundSlot?.Id.ToString();

        // Unity event invoked when stack count text is changed.
        [SerializeField] private StringUnityEvent onStackCountChanged = default;

        // Instance of the currently assigned item contained inside the slot UI.
        private InventoryItem item;
        
        public void Bind (InventorySlot inventorySlot)
        {
            // For performance reasons, instead of spawning UIs for all the slots in the grid,
            // we spawn only the slots visible to the player and change their state
            // according to the bound slot model and selected grid page.
            
            // Unsubscribe from the events of the previously bound model.
            if (BoundSlot != null)
            {
                BoundSlot.OnItemChanged -= SetItem;
                BoundSlot.OnStackCountChanged -= SetStackCount;
            }

            // Store reference to the new slot model.
            BoundSlot = inventorySlot;
            
            // Receive updates when bound slot model is modified.
            BoundSlot.OnItemChanged += SetItem;
            BoundSlot.OnStackCountChanged += SetStackCount;

            // Update UI state for the new slot model.
            SetItem(inventorySlot.Item);
            SetStackCount(inventorySlot.StackCount);
        }

        private void SetItem (InventoryItem prototype)
        {
            if (ObjectUtils.IsValid(prototype))
            {
                // Note that this naive implementation is just for example.
                // In real projects use object pool instead of instantiating and destroying the items.
                if (item) ObjectUtils.DestroyOrImmediate(item.gameObject);
                item = Instantiate(prototype, transform, false);
                item.transform.SetAsFirstSibling(); // this will make stack count text render on top of the item
            }
            else SetEmpty();
        }

        private void SetStackCount (int count)
        {
            var text = count <= 1 ? string.Empty : count.ToString();
            onStackCountChanged?.Invoke(text);
        }

        private void SetEmpty ()
        {
            if (item) ObjectUtils.DestroyOrImmediate(item.gameObject);
            SetStackCount(0);
        }
    }
}
