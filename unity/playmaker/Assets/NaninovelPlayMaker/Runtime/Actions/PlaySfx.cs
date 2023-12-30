using HutongGames.PlayMaker;

namespace Naninovel.PlayMaker
{
    [ActionCategory("Naninovel")]
    public class PlaySfx : FsmStateAction
    {
        [Tooltip("The name (path) of the SFX resource to play.")]
        [UIHint(UIHint.FsmString)]
        public FsmString SfxName;

        [Tooltip("Whether to loop the SFX playback.")]
        [UIHint(UIHint.FsmBool)]
        public FsmBool Loop;

        [Tooltip("The volume of the playback, in 0.0 to 1.0 range.")]
        [UIHint(UIHint.FsmString)]
        public FsmFloat Volume;

        [Tooltip("The duration (in seconds) of the fade-in.")]
        [UIHint(UIHint.FsmString)]
        public FsmFloat FadeTime;

        [Tooltip("Whether to play the SFX without saving state and clipping any currently played SFX with the same name. Use for short audio clips played repeatedly (eg, UI button hover/clicking sounds).")]
        [UIHint(UIHint.FsmBool)]
        public FsmBool Fast;

        [Tooltip("Event sent when the SFX has started playing. Could be delayed for the duration of the fade-in or when the audio clip resource is used first time and should be loaded before playback is possible.")]
        public FsmEvent DoneEvent;

        public override void Reset ()
        {
            SfxName = null;
            Loop = null;
            Volume = 1;
            FadeTime = null;
            Fast = null;
            DoneEvent = null;
        }

        public override void OnEnter ()
        {
            if (!Engine.Initialized)
            {
                UnityEngine.Debug.LogError("Can't play SFX: Naninovel engine is not initialized.");
                Finish();
                return;
            }

            PreloadAndPlaySfxAsync();
        }

        private async void PreloadAndPlaySfxAsync ()
        {
            var audioManager = Engine.GetService<AudioManager>();

            await audioManager.AudioLoader.LoadAndHoldAsync(SfxName.Value, this);

            if (Fast.Value) audioManager.PlaySfxFast(SfxName.Value, Volume.Value);
            else await audioManager.PlaySfxAsync(SfxName.Value, Volume.Value, FadeTime.Value, Loop.Value);

            Fsm.Event(DoneEvent);
            Finish();
        }

        public override void OnExit ()
        {
            base.OnExit();

            Engine.GetService<AudioManager>()?.AudioLoader?.Release(SfxName.Value, this);
        }
    }
}
