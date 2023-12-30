using Naninovel;
using UnityEngine;

[ActorResources(typeof(Texture2D), true)]
public class SpriteRootBackground : SpriteBackground
{
    public SpriteRootBackground (string id, BackgroundMetadata metadata)
        : base(id, metadata) { }

    protected override LocalizableResourceLoader<Texture2D> ConstructAppearanceLoader (OrthoActorMetadata metadata)
    {
        // Override the path prefix of the loader so that background sprites
        // can be loaded from the root of the `/Backgrounds` folder.
        var providerManager = Engine.GetService<IResourceProviderManager>();
        var localeManager = Engine.GetService<ILocalizationManager>();
        var appearanceLoader = new LocalizableResourceLoader<Texture2D>(
            providerManager.GetProviders(metadata.Loader.ProviderTypes),
            providerManager, localeManager, metadata.Loader.PathPrefix);

        return appearanceLoader;
    }
}
