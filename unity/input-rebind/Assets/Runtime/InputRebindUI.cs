using Naninovel;
using Naninovel.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputRebindUI : CustomUI
{
    [SerializeField] private InputActionAsset actions;

    private const string prefsKey = "NaninovelInputRebinds";
    private IInputManager input => Engine.GetService<IInputManager>();

    protected override void Awake ()
    {
        base.Awake();
        this.AssertRequiredObjects(actions);
    }

    protected override void HandleVisibilityChanged (bool visible)
    {
        base.HandleVisibilityChanged(visible);

        if (input.TryGetSampler(InputNames.Cancel, out var cancel))
            if (visible) cancel.OnEnd += Hide;
            else cancel.OnEnd -= Hide;

        if (visible) LoadBindings();
        else SaveBindings();
    }

    private void SaveBindings ()
    {
        var rebinds = actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString(prefsKey, rebinds);
    }

    private void LoadBindings ()
    {
        var rebinds = PlayerPrefs.GetString(prefsKey);
        if (!string.IsNullOrEmpty(rebinds))
            actions.LoadBindingOverridesFromJson(rebinds);
    }
}
