using Naninovel;
using UnityEngine;

public class InitializeWithUniTask : MonoBehaviour
{
    // ReSharper disable once Unity.IncorrectMethodSignature
    private async Cysharp.Threading.Tasks.UniTaskVoid Start ()
    {
        await RuntimeInitializer.InitializeAsync();
    }
}
