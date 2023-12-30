using UnityEngine.InputSystem;

public static class EnableGyroscope
{
    /// <summary>
    /// Gyroscope device is disabled by default in the Unity's input system,
    /// so we have to manually enable it.
    /// </summary>
    [UnityEngine.RuntimeInitializeOnLoadMethod]
    private static void EnableGyroscopeOnInit ()
    {
        if (Gyroscope.current != null)
            InputSystem.EnableDevice(Gyroscope.current);
    }
}
