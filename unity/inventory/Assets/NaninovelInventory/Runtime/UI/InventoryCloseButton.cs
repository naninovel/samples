using Naninovel;

namespace NaninovelInventory
{
    /// <summary>
    /// Attached to the game object that hosts button to close (hide) inventory UI.
    /// </summary>
    public class InventoryCloseButton : ScriptableLabeledButton
    {
        private InventoryUI inventoryUI;

        protected override void Awake ()
        {
            base.Awake();

            inventoryUI = GetComponentInParent<InventoryUI>();
        }

        protected override void OnButtonClick () => inventoryUI.Hide();
    }
}
