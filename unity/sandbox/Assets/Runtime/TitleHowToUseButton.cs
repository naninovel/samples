using Naninovel;

public class TitleHowToUseButton : ScriptableButton
{
    private UIManager uiManager;

    protected override void Awake ()
    {
        base.Awake();

        uiManager = Engine.GetService<UIManager>();
    }

    protected override void OnButtonClick ()
    {
        var howToUI = uiManager.GetUI<IHowToUseUI>();
        if (howToUI == null) return;

        howToUI.Show();
    }
}
