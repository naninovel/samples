using Naninovel;
using UnityEngine.UI;

public class HowToUseUploadButton : ScriptableButton
{
    private HowToUsePanel howToUsePanel;
    private UIManager uiManager;
    private Text label;
    private string initialLabelText;

    protected override void Awake ()
    {
        base.Awake();

        uiManager = Engine.GetService<UIManager>();
        label = GetComponentInChildren<Text>();
        howToUsePanel = GetComponentInParent<HowToUsePanel>();

        initialLabelText = label.text;
    }

    protected override void OnEnable ()
    {
        base.OnEnable();

        howToUsePanel.OnVisibilityChanged += HandlePanelVisibilityChanged;
    }

    protected override void OnDisable ()
    {
        base.OnDisable();

        howToUsePanel.OnVisibilityChanged -= HandlePanelVisibilityChanged;
    }

    protected override void OnButtonClick ()
    {
        var howToUseUI = uiManager.GetUI<IHowToUseUI>();
        if (howToUseUI == null || howToUseUI.UploadProgress > 0) return;

        HandleDataUploadAsync(howToUseUI);
    }

    private void HandlePanelVisibilityChanged (bool isVisible)
    {
        if (!isVisible) return;

        label.text = initialLabelText;
        SetInteractable(true);
    }

    private async void HandleDataUploadAsync (IHowToUseUI howToUseUI)
    {
        SetInteractable(false);

        howToUseUI.OnUploadProgress += HandleUploadProgress;
        var success = await howToUseUI.UploadDataAsync();
        howToUseUI.OnUploadProgress -= HandleUploadProgress;

        label.text = success ? "DATA UPLOADED" : "UPLOAD FAILED";
    }

    private void HandleUploadProgress (float progress)
    {
        label.text = $"UPLOADING: {progress:P0} COMPLETE";
    }
}
