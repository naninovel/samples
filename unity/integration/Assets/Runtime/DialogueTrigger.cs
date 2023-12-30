using UnityEngine;
using UnityCommon;
using Naninovel;

public class DialogueTrigger : MonoBehaviour
{
    public string ScriptName;
    public string Label;

    private void OnTriggerEnter (Collider other)
    {
        var controller = other.gameObject.GetComponentInChildren<CharacterController3D>();
        controller.IsInputBlocked = true;

        var inputManager = Engine.GetService<IInputManager>();
        inputManager.ProcessInput = true;

        var scriptPlayer = Engine.GetService<IScriptPlayer>();
        scriptPlayer.PreloadAndPlayAsync(ScriptName, label: Label).Forget();
    }
}
