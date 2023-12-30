using HutongGames.PlayMaker;

namespace Naninovel.PlayMaker
{
    [ActionCategory("Naninovel")]
    public class ResetState : FsmStateAction
    {
        public override void OnEnter ()
        {
            DoAsync();
        }

        private async void DoAsync ()
        {
            await Engine.GetService<StateManager>()?.ResetStateAsync();

            Finish();
        }
    }
}
