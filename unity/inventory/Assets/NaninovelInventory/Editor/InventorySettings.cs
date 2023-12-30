using Naninovel;
using System;
using UnityEditor;

namespace NaninovelInventory
{
    public class InventorySettings : ResourcefulSettings<InventoryConfiguration>
    {
        protected override Type ResourcesTypeConstraint => typeof(InventoryItem);
        protected override string ResourcesCategoryId => Configuration.Loader.PathPrefix;
        protected override string ResourcesSelectionTooltip => "Use `@addItem %name%` to add the item to the player inventory.";

        [MenuItem("Naninovel/Resources/Inventory")]
        private static void OpenResourcesWindow () => OpenResourcesWindowImpl();
    }
}