using HutongGames.PlayMaker;
using System.Globalization;

namespace Naninovel.PlayMaker
{
    [ActionCategory("Naninovel")]
    public class GetCustomVariable : FsmStateAction
    {
        [Tooltip("The name of the variable to get.")]
        [UIHint(UIHint.FsmString)]
        public FsmString VariableName;

        [Tooltip("The retrieved string value of the variable.")]
        [UIHint(UIHint.Variable)]
        public FsmString StringVariableValue;

        [Tooltip("The retrieved float value of the variable.")]
        [UIHint(UIHint.Variable)]
        public FsmFloat FloatVariableValue;

        [Tooltip("The retrieved int value of the variable.")]
        [UIHint(UIHint.Variable)]
        public FsmInt IntVariableValue;

        [Tooltip("The retrieved bool value of the variable.")]
        [UIHint(UIHint.Variable)]
        public FsmBool BoolVariableValue;

        public override void Reset ()
        {
            VariableName = null;
            StringVariableValue = null;
            FloatVariableValue = null;
            IntVariableValue = null;
            BoolVariableValue = null;
        }

        public override void OnEnter ()
        {
            var customVarManager = Engine.GetService<CustomVariableManager>();
            if (customVarManager is null) { Finish(); return; }

            StringVariableValue.Value = customVarManager.GetVariableValue(VariableName.Value);

            if (!string.IsNullOrEmpty(StringVariableValue.Value) && float.TryParse(StringVariableValue.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var floatValue))
                FloatVariableValue.Value = floatValue;
            if (!string.IsNullOrEmpty(StringVariableValue.Value) && int.TryParse(StringVariableValue.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intValue))
                IntVariableValue.Value = intValue;
            if (!string.IsNullOrEmpty(StringVariableValue.Value) && bool.TryParse(StringVariableValue.Value, out var boolValue))
                BoolVariableValue.Value = boolValue;

            Finish();
        }
    }
}
