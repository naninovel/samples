using HutongGames.PlayMaker;

namespace Naninovel.PlayMaker
{
    [ActionCategory("Naninovel")]
    public class GetScriptPlayerStatus : FsmStateAction
    {
        [Tooltip("Whether the player is currently playing a script.")]
        [UIHint(UIHint.Variable)]
        public FsmBool IsPlaying;

        [Tooltip("Whether skip mode is currently active.")]
        [UIHint(UIHint.Variable)]
        public FsmBool IsSkipActive;

        [Tooltip("Whether auto play mode is currently active.")]
        [UIHint(UIHint.Variable)]
        public FsmBool IsAutoPlayActive;

        [Tooltip("Whether user input is required to execute next script command.")]
        [UIHint(UIHint.Variable)]
        public FsmBool IsWaitingForInput;

        [Tooltip("Name of the currently played scenario script.")]
        [UIHint(UIHint.Variable)]
        public FsmString PlayedScriptName;

        [Tooltip("ID of the currently played script command.")]
        [UIHint(UIHint.Variable)]
        public FsmString PlayedCommandId;

        [Tooltip("Index of the currently played command.")]
        [UIHint(UIHint.Variable)]
        public FsmInt PlayedCommandIndex;

        [Tooltip("Total number of commands in the currently played script.")]
        [UIHint(UIHint.Variable)]
        public FsmInt TotalCommandsCount;


        public override void Reset ()
        {
            IsPlaying = null;
            IsSkipActive = null;
            IsAutoPlayActive = null;
            IsWaitingForInput = null;
            PlayedScriptName = null;
            PlayedCommandId = null;
            PlayedCommandIndex = null;
            TotalCommandsCount = null;
        }

        public override void OnEnter ()
        {
            var player = Engine.GetService<ScriptPlayer>();
            if (player is null) { Finish(); return; }

            IsPlaying.Value = player.Playing;
            IsSkipActive.Value = player.SkipActive;
            IsAutoPlayActive.Value = player.AutoPlayActive;
            IsWaitingForInput.Value = player.WaitingForInput;
            PlayedScriptName.Value = player.PlayedScript?.Name;
            PlayedCommandId.Value = player.PlayedCommand?.GetType()?.Name;
            PlayedCommandIndex.Value = player.PlayedIndex;
            TotalCommandsCount.Value = player.Playlist?.Count ?? 0;

            Finish();
        }
    }
}
