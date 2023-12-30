using System;
using Naninovel;
using UnityEngine;

[CommandAlias("uniTask")]
public class UniTaskCommand : Command
{
    [ParameterAlias(NamelessParameterAlias), RequiredParameter]
    public StringParameter Message;

    public override async UniTask ExecuteAsync (AsyncToken asyncToken = default)
    {
        var message = await WaitAndReturnMessageAsync();
        Debug.Log(message);
    }

    private async Cysharp.Threading.Tasks.UniTask<string> WaitAndReturnMessageAsync ()
    {
        await Cysharp.Threading.Tasks.UniTask.Delay(TimeSpan.FromSeconds(1));
        return Message;
    }
}
