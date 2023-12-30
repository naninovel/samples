using Naninovel;
using Naninovel.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIDocumentAdapter : CustomUI
{
    protected virtual UIDocument Document => document;
    protected virtual RawImage RenderImage => renderImage;

    [SerializeField] private UIDocument document;
    [SerializeField] private RawImage renderImage;

    public override void SetVisibility (bool visible)
    {
        base.SetVisibility(visible);
        Document.rootVisualElement.visible = visible;
    }

    public override async UniTask ChangeVisibilityAsync (bool visible, float? duration = null,
        AsyncToken asyncToken = default)
    {
        if (visible) Document.rootVisualElement.visible = true;
        await base.ChangeVisibilityAsync(visible, duration, asyncToken);
        if (!visible) Document.rootVisualElement.visible = false;
    }

    protected override void Awake ()
    {
        base.Awake();
        SetupDocumentRendering();
    }

    protected virtual void SetupDocumentRendering ()
    {
        this.AssertRequiredObjects(Document, RenderImage);
        var renderTexture = CreateRenderTexture();
        RenderImage.texture = renderTexture;
        Document.panelSettings.targetTexture = renderTexture;
        Document.panelSettings.clearColor = true;
    }

    protected virtual RenderTexture CreateRenderTexture ()
    {
        var width = (int)TopmostCanvas.pixelRect.width;
        var height = (int)TopmostCanvas.pixelRect.height;
        return new RenderTexture(width, height, 24);
    }

    protected virtual void DestroyRenderTexture ()
    {
        if (RenderImage && RenderImage.texture)
            ObjectUtils.DestroyOrImmediate(RenderImage.texture);
    }

    protected override void OnDestroy ()
    {
        base.OnDestroy();
        DestroyRenderTexture();
    }
}
