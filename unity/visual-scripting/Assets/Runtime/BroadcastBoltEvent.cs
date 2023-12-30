using Naninovel;
using Unity.VisualScripting;
using UnityEngine;

[CommandAlias("bolt")]
public class BroadcastBoltEvent : Command
{
    [ParameterAlias("object"), RequiredParameter]
    public StringParameter GameObjectName;
    [ParameterAlias("name"), RequiredParameter]
    public StringParameter EventName;
    [ParameterAlias("args")]
    public StringListParameter Arguments;

    public override UniTask ExecuteAsync (AsyncToken asyncToken = default)
    {
        var gameObject = GameObject.Find(GameObjectName);
        if (gameObject == null)
        {
            Debug.LogError($"Failed to broadcast `{EventName}` bolt event: `{GameObjectName}` game object is not found.");
            return UniTask.CompletedTask;
        }

        CustomEvent.Trigger(gameObject, EventName, Arguments);

        return UniTask.CompletedTask;
    }
}
