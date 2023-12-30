using Naninovel.UI;
using UnityEngine;

public class ExitButton : TitleExitButton
{
    protected override void OnButtonClick ()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
            Application.OpenURL("https://naninovel.com");
        else Application.Quit();
    }
}
