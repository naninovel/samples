using System.Linq;
using AC;
using UnityEngine;

namespace Naninovel.AC
{
    /// <summary>
    /// Enables Adventure Creator and (optionally) resets Naninovel engine.
    /// </summary>
    public class TurnOnAC : Command
    {
        /// <summary>
        /// Whether to reset the state and stop all the Naninovel engine services.
        /// </summary>
        public BooleanParameter Reset = true;
        /// <summary>
        /// Whether to disable Naninovel's camera and enable Adventure Creator's main camera.
        /// </summary>
        public BooleanParameter SwapCameras = true;
        /// <summary>
        /// Name of the action list (game object) to play after turning on Adventure Creator.
        /// </summary>
        public StringParameter Action;

        public override async UniTask ExecuteAsync (AsyncToken asyncToken = default)
        {
            if (Reset) await Engine.GetService<StateManager>().ResetStateAsync();

            KickStarter.TurnOnAC();

            if (SwapCameras)
            {
                KickStarter.mainCamera.enabled = true;
                Engine.GetService<CameraManager>().Camera.enabled = false;
            }

            if (Assigned(Action))
            {
                var actionList = Object.FindObjectsOfType<ActionList>().FirstOrDefault(a => a.gameObject.name == Action);
                if (actionList is null)
                {
                    Err($"Failed to play `{Action}` Adventure Creator action list: action list with the name not found on scene.");
                    return;
                }
                actionList.RunFromIndex(0);
            }
        }
    }
}
