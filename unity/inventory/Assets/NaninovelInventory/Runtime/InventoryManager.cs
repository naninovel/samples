using System;
using Naninovel;
using UnityEngine;

namespace NaninovelInventory
{
    /// <summary>
    /// A custom engine service used to manage available inventory items.
    /// </summary>
    [InitializeAtRuntime] // makes the service auto-initialize with other built-in engine services
    [Naninovel.Commands.Goto.DontReset] // skips the service reset on @goto commands (to preserve the inventory when navigating scripts)
    public class InventoryManager : IEngineService<InventoryConfiguration>
    {
        public InventoryConfiguration Configuration { get; }

        private readonly IResourceProviderManager providersManager;
        private readonly ILocalizationManager localizationManager;
        private LocalizableResourceLoader<GameObject> itemLoader;

        public InventoryManager (InventoryConfiguration config,
            IResourceProviderManager providersManager, ILocalizationManager localizationManager)
        {
            // Engine service constructors are invoked when the engine is initializing;
            // remember that it's not safe to use other services here, as they are not initialized yet.
            // Instead, store references to the required services and use them in `InitializeServiceAsync()` method.

            Configuration = config;
            this.providersManager = providersManager; // required to load item prefabs
            this.localizationManager = localizationManager; // required to load localized versions of item prefabs
        }

        public UniTask InitializeServiceAsync ()
        {
            // Invoked when the engine is initializing, after services required in the constructor are initialized;
            // it's safe to use the required services here (IResourceProviderManager in this case).

            // Initialize item prefab loader, as per the configuration.
            itemLoader = Configuration.Loader.CreateLocalizableFor<GameObject>(providersManager, localizationManager);

            return UniTask.CompletedTask;
        }

        public void ResetService ()
        {
            // Invoked when resetting engine state (eg, loading a script or starting a new game).

            var inventory = Engine.GetService<IUIManager>().GetUI<InventoryUI>();
            if (ObjectUtils.IsValid(inventory))
                inventory.RemoveAllItems(); // remove all items from the current inventory

            itemLoader?.ReleaseAll(this); // unload item prefabs to free memory
        }

        public void DestroyService ()
        {
            // Invoked when destroying the engine.

            itemLoader?.ReleaseAll(this);
        }

        /// <summary>
        /// Attempts to retrieve item prefab with the specified identifier.
        /// </summary>
        public async UniTask<InventoryItem> GetItemAsync (string itemId)
        {
            // If item resource is already loaded, get it; otherwise load asynchronously.
            var itemResource = itemLoader.GetLoadedOrNull(itemId) ?? await itemLoader.LoadAndHoldAsync(itemId, this);
            if (!itemResource.Valid) throw new Exception($"Failed to load `{itemId}` item resource.");
            return itemResource.Object.GetComponent<InventoryItem>();
        }
    }
}
