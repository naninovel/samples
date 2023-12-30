using Naninovel;

namespace NaninovelInventory
{
    [ExpressionFunctions]
    public static class InventoryFunctions
    {
        public static int ItemCount (string itemId)
        {
            var uiManager = Engine.GetService<IUIManager>();
            var inventory = uiManager.GetUI<InventoryUI>();
            return inventory.CountItem(itemId);
        }

        public static bool ItemExist (string itemId) => ItemCount(itemId) > 0;
    }
}
