using Naninovel;

public class HowToUseReturnButton : ScriptableButton
{
    private HowToUsePanel howToUsePanel;

    protected override void Awake ()
    {
        base.Awake();

        howToUsePanel = GetComponentInParent<HowToUsePanel>();
    }

    protected override void OnButtonClick () => howToUsePanel.Hide();
}
