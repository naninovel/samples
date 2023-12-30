using HutongGames.PlayMaker;

namespace Naninovel.PlayMaker
{
    [ActionCategory("Naninovel")]
    public class SetCustomVariable : FsmStateAction
    {
        [Tooltip("The name of the variable to set.")]
        [UIHint(UIHint.FsmString)]
        public FsmString VariableName;

        [Tooltip("The value of the variable to set.")]
        [UIHint(UIHint.Variable)]
        public FsmString VariableValue;

        public override void Reset ()
        {
            VariableName = null;
            VariableValue = null;
        }

        public override void OnEnter ()
        {
            var customVarManager = Engine.GetService<CustomVariableManager>();
            if (customVarManager is null) { Finish(); return; }

            customVarManager.SetVariableValue(VariableName.Value, VariableValue.Value);

            Finish();
        }
    }
}
