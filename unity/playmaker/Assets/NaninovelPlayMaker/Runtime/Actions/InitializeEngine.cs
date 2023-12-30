using HutongGames.PlayMaker;

namespace Naninovel.PlayMaker
{
    [ActionCategory("Naninovel")]
    public class InitializeEngine : FsmStateAction
    {
        [Tooltip("Whether the initalization has finished.")]
        [UIHint(UIHint.Variable)]
        public FsmBool IsDone;

        [Tooltip("Event sent when the initalization is finished.")]
        public FsmEvent DoneEvent;

        public override void Reset ()
        {
            IsDone = null;
            DoneEvent = null;
        }

        public override void OnEnter ()
        {
            IsDone.Value = false;

            InitializeEngineAsync();
        }

        private async void InitializeEngineAsync ()
        {
            await RuntimeInitializer.InitializeAsync();

            IsDone.Value = true;
            Fsm.Event(DoneEvent);
            Finish();
        }
    }
}
