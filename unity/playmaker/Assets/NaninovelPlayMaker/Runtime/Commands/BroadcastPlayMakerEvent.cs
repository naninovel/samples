using System.Linq;

namespace Naninovel.PlayMaker
{
    /// <summary>
    /// Broadcasts a PlayMaker event with the provided name.
    /// When neither `fsm` nor `object` params are specified, the event will be broadcasted globally to all the active FSMs on scene.
    /// </summary>
    [CommandAlias("playmaker")]
    public class BroadcastPlayMakerEvent : Command
    {
        /// <summary>
        /// Name of the event to broadcast.
        /// </summary>
        [ParameterAlias(NamelessParameterAlias), RequiredParameter]
        public StringParameter EventName;
        /// <summary>
        /// Names of FSMs for which to broadcast the event.
        /// </summary>
        [ParameterAlias("fsm")]
        public StringListParameter FsmNames;
        /// <summary>
        /// Names of game objects for which to broadcast the event. The objects should have an FSM component attached.
        /// </summary>
        [ParameterAlias("object")]
        public StringListParameter GameObjectNames;

        public override UniTask ExecuteAsync (AsyncToken asyncToken = default)
        {
            if (!Assigned(FsmNames) && !Assigned(GameObjectNames))
            {
                PlayMakerFSM.BroadcastEvent(EventName);
                return UniTask.CompletedTask;
            }

            var fsmNames = Assigned(FsmNames) ? FsmNames.ToList() : null;
            var objectNames = Assigned(GameObjectNames) ? GameObjectNames.ToList() : null;
            foreach (var fsm in PlayMakerFSM.FsmList)
            {
                if (objectNames != null && !objectNames.Contains(fsm.gameObject.name)) continue;
                if (fsmNames != null && !fsmNames.Contains(fsm.FsmName)) continue;

                fsm.SendEvent(EventName);
            }

            return UniTask.CompletedTask;
        }
    }
}
