using HutongGames.PlayMaker;

namespace Naninovel.PlayMaker
{
    [ActionCategory("Naninovel")]
    public class DestroyEngine : FsmStateAction
    {
        public override void OnEnter ()
        {
            Engine.Destroy();
            Finish();
        }
    }
}
