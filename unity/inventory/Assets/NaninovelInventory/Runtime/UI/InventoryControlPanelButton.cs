using Naninovel;

namespace NaninovelInventory
{
    public class InventoryControlPanelButton : ScriptableLabeledButton
    {
        private InventoryUI inventory;

        protected override void Awake ()
        {
            base.Awake();

            inventory = Engine.GetService<IUIManager>().GetUI<InventoryUI>();
        }

        protected override void OnButtonClick () => inventory.ToggleVisibility();
    }
}
