using System.Collections;
using Naninovel;
using UnityEngine;
using UnityGoogleDrive;

public class InitializeEngine : MonoBehaviour
{
    private IEnumerator Start ()
    {
        AuthController.RefreshAccessToken();
        while (AuthController.IsRefreshingAccessToken)
            yield return null;
        RuntimeInitializer.InitializeAsync().Forget();
    }
}
