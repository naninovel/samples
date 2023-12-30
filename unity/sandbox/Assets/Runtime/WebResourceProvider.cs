using Naninovel;

[InitializeAtRuntime(@override: typeof(ResourceProviderManager))]
public class WebResourceProvider : ResourceProviderManager
{
    public WebResourceProvider (ResourceProviderConfiguration config)
        : base(config) { }

    protected override IResourceProvider InitializeGoogleDriveProvider ()
    {
        var provider = (GoogleDriveResourceProvider)base.InitializeGoogleDriveProvider();
        provider.AddConverter(new Mp3ToAudioClipConverter());
        return provider;
    }
}
