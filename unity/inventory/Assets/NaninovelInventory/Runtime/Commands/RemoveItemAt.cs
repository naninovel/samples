using Naninovel;

namespace NaninovelInventory
{
    [Documentation("Removes item from an inventory slot with the specified identifier.")]
    public class RemoveItemAt : Command
    {
        [RequiredParameter, ParameterAlias(NamelessParameterAlias), Documentation("Identifier of inventory slot to remove item from.")]
        public IntegerParameter SlotId;
        [Documentation("Number of items to remove.")]
        public IntegerParameter Amount = 1;

        public override UniTask ExecuteAsync (AsyncToken asyncToken = default)
        {
            var uiManager = Engine.GetService<IUIManager>();
            var inventory = uiManager.GetUI<InventoryUI>();

            inventory.RemoveItemAt(SlotId, Amount);

            return UniTask.CompletedTask;
        }
    }
}
