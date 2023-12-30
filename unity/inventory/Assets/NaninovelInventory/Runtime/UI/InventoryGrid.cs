using System;
using System.Collections.Generic;
using Naninovel;

namespace NaninovelInventory
{
    /// <summary>
    /// Attached to the game object, that hosts <see cref="UnityEngine.UI.GridLayoutGroup"/>.
    /// Provides several utility methods to work with the grid layout (via <see cref="ScriptableGrid{TSlot}"/>).
    /// </summary>
    public class InventoryGrid : ScriptableGrid<InventoryGridSlot>
    {
        public Action<int> OnSlotClicked;

        public IReadOnlyList<InventorySlot> InventorySlots { get; private set; }

        public override void Initialize (int itemsCount)
        {
            var slots = new InventorySlot[itemsCount];
            for (int i = 0; i < itemsCount; i++)
                slots[i] = new InventorySlot(i);
            InventorySlots = slots;

            base.Initialize(itemsCount);
        }

        protected override void InitializeSlot (InventoryGridSlot gridSlot)
        {
            gridSlot.OnButtonClicked += () => OnSlotClicked?.Invoke(gridSlot.BoundSlot?.Id ?? -1);
        }

        protected override void BindSlot (InventoryGridSlot gridSlot, int itemIndex)
        {
            var inventorySlot = InventorySlots[itemIndex];
            gridSlot.Bind(inventorySlot);
        }
    }
}
