using Naninovel;

namespace NaninovelInventory
{
    [Documentation("Removes all item in the inventory.")]
    public class RemoveAllItems : Command
    {
        public override UniTask ExecuteAsync (AsyncToken asyncToken = default)
        {
            var uiManager = Engine.GetService<IUIManager>();
            var inventory = uiManager.GetUI<InventoryUI>();

            inventory.RemoveAllItems();

            return UniTask.CompletedTask;
        }
    }
}
