using Naninovel;

namespace NaninovelInventory
{
    [Documentation("Removes item with the specified identifier from the inventory.")]
    public class RemoveItem : Command
    {
        [RequiredParameter, ParameterAlias(NamelessParameterAlias), Documentation("Identifier of the item to remove.")]
        public StringParameter ItemId;
        [Documentation("Number of items to remove.")]
        public IntegerParameter Amount = 1;

        public override UniTask ExecuteAsync (AsyncToken asyncToken = default)
        {
            var uiManager = Engine.GetService<IUIManager>();
            var inventory = uiManager.GetUI<InventoryUI>();

            inventory.RemoveItem(ItemId, Amount);

            return UniTask.CompletedTask;
        }
    }
}
