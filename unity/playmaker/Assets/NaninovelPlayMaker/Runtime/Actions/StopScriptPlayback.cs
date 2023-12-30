using HutongGames.PlayMaker;

namespace Naninovel.PlayMaker
{
    [ActionCategory("Naninovel")]
    public class StopScriptPlayback : FsmStateAction
    {
        public override void OnEnter ()
        {
            Engine.GetService<ScriptPlayer>()?.Stop();
            Finish();
        }
    }
}
