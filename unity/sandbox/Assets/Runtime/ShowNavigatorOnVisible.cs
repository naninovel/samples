using UnityEngine;
using Naninovel;
using Naninovel.UI;

[RequireComponent(typeof(ScriptableUIBehaviour))]
public class ShowNavigatorOnVisible : MonoBehaviour
{
    private ScriptableUIBehaviour scriptableUIBehaviour;

    private void Awake ()
    {
        scriptableUIBehaviour = GetComponent<ScriptableUIBehaviour>();
    }

    private void OnEnable ()
    {
        scriptableUIBehaviour.OnVisibilityChanged += HandleVisibilityChanged;
    }

    private void OnDisable ()
    {
        scriptableUIBehaviour.OnVisibilityChanged -= HandleVisibilityChanged;
    }

    private void HandleVisibilityChanged (bool isVisible)
    {
        if (isVisible) Engine.GetService<IUIManager>()?.GetUI<IScriptNavigatorUI>()?.Show();
    }
}
