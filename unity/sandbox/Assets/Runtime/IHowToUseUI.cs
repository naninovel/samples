using System;
using Naninovel;
using Naninovel.UI;

public interface IHowToUseUI : IManagedUI
{
    event Action<float> OnUploadProgress;

    float UploadProgress { get; }

    UniTask<bool> UploadDataAsync ();
}
