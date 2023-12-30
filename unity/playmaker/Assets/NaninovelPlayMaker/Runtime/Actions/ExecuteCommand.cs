using HutongGames.PlayMaker;

namespace Naninovel.PlayMaker
{
    [ActionCategory("Naninovel")]
    public class ExecuteCommand : FsmStateAction
    {
        [Tooltip("The text of the script command to execute.")]
        [UIHint(UIHint.FsmString)]
        public FsmString CommandText;

        public override void Reset ()
        {
            CommandText = null;
        }

        public override void OnEnter ()
        {
            DoAsync();
        }

        private async void DoAsync ()
        {
            if (string.IsNullOrEmpty(CommandText.Value))
            {
                Finish();
                return;
            }

            if (!Engine.Initialized) throw new Error("Failed to execute command: Naninovel is not initialized.");
            
            await Engine.GetService<IScriptPlayer>().PlayTransient("PlayMaker", CommandText.Value);

            Finish();
        }
    }
}
