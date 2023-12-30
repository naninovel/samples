using UnityEngine;
using Naninovel;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AC
{
    [System.Serializable]
    public class ActionPlayNaninovelScript : Action
    {
        public string ScriptName = string.Empty;
        public string Label = string.Empty;
        public bool TurnOffAC = true;
        public bool SwapCameras = true;

        private bool asyncCompleted;

        public ActionPlayNaninovelScript ()
        {
            isDisplayed = true;
            category = ActionCategory.Custom;
            title = "Play Naninovel Script";
            description = "Turn off AC (optionally), initialize Naninovel engine (if necessary) and play a Naninovel script with the specified name.";
        }

        public override float Run ()
        {
            if (!isRunning)
            {
                asyncCompleted = false;
                isRunning = true;
                RunAsync();
            }
            else if (asyncCompleted)
            {
                isRunning = false;
                return 0f;
            }

            return defaultPauseTime;
        }

        public override void Skip ()
        {
            /*
			 * This function is called when the Action is skipped, as a
			 * result of the player invoking the "EndCutscene" input.
			 * 
			 * It should perform the instructions of the Action instantly -
			 * regardless of whether or not the Action itself has been run
			 * normally yet.  If this method is left blank, then skipping
			 * the Action will have no effect.  If this method is removed,
			 * or if the Run() method call is left below, then skipping the
			 * Action will cause it to run itself as normal.
			 */

            Run();
        }

        #if UNITY_EDITOR

        public override void ShowGUI ()
        {
            ScriptName = EditorGUILayout.TextField(new GUIContent("Script Name:", "Name of the Naninovel script to play."), ScriptName);
            Label = EditorGUILayout.TextField(new GUIContent("Label (optional):", "Label inside the specified Naninovel script from which to start playing (optional)."), Label);
            TurnOffAC = EditorGUILayout.Toggle(new GUIContent("Turn OFF AC:", "Whether to turn off (disable) Adventure Creator before playing the script. You can use @turnOnAC Naninovel command to enable it again from a Naninovel script."), TurnOffAC);
            SwapCameras = EditorGUILayout.Toggle(new GUIContent("Swap Cameras:", "Whether to disable Adventure Creator's main camera and enable Naninovel's camera."), SwapCameras);

            AfterRunningOption();
        }

        public override string SetLabel () => $"{(TurnOffAC ? "Turn off AC" : string.Empty)}, initialize Naninovel engine (if necessary) and play a `{ScriptName}` Naninovel script{(string.IsNullOrWhiteSpace(Label) ? string.Empty : $"starting at `{Label}` label")}.";

        #endif

        private async void RunAsync ()
        {
            if (string.IsNullOrWhiteSpace(ScriptName))
            {
                Debug.LogError("Can't run Naninovel script from AC action: script name is not specified.");
                asyncCompleted = true;
                return;
            }

            if (TurnOffAC)
                KickStarter.TurnOffAC();

            if (!Engine.Initialized)
                await RuntimeInitializer.InitializeAsync();

            if (SwapCameras)
            {
                KickStarter.mainCamera.enabled = false;
                Engine.GetService<CameraManager>().Camera.enabled = true;
            }

            await Engine.GetService<ScriptPlayer>().PreloadAndPlayAsync(ScriptName, label: Label);

            asyncCompleted = true;
        }
    }
}
