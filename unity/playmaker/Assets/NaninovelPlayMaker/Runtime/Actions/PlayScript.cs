using HutongGames.PlayMaker;

namespace Naninovel.PlayMaker
{
    [ActionCategory("Naninovel")]
    public class PlayScript : FsmStateAction
    {
        [Tooltip("The name of the script play.")]
        [UIHint(UIHint.FsmString)]
        public FsmString ScriptName;

        [Tooltip("The label inside the target script from which to start the playback (optional).")]
        [UIHint(UIHint.FsmString)]
        public FsmString Label;

        [Tooltip("Event sent when the script has started playing.")]
        public FsmEvent DoneEvent;

        public override void Reset ()
        {
            ScriptName = null;
            Label = null;
            DoneEvent = null;
        }

        public override void OnEnter ()
        {
            if (!Engine.Initialized)
            {
                UnityEngine.Debug.LogError("Can't play script: Naninovel engine is not initialized.");
                Finish();
                return;
            }

            PreloadAndPlayScriptAsync();
        }

        private async void PreloadAndPlayScriptAsync ()
        {
            var player = Engine.GetService<ScriptPlayer>();
            var label = string.IsNullOrWhiteSpace(Label?.Value) ? null : Label.Value;
            await player.PreloadAndPlayAsync(ScriptName.Value, label: label);

            Fsm.Event(DoneEvent);
            Finish();
        }
    }
}
