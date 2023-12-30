using HutongGames.PlayMaker;

namespace Naninovel.PlayMaker
{
    /// <summary>
    /// Sets a global playmaker variable with the provided name.
    /// </summary>
    [CommandAlias("pset")]
    public class SetPlayMakerVariable : Command
    {
        /// <summary>
        /// Name of the variable to set.
        /// </summary>
        [ParameterAlias("name"), RequiredParameter]
        public StringParameter VariableName;
        /// <summary>
        /// Value of the variable to set.
        /// </summary>
        [ParameterAlias("value"), RequiredParameter]
        public StringParameter VariableValue;
        /// <summary>
        /// When setting an array variable, provide index here.
        /// </summary>
        [ParameterAlias("index")]
        public IntegerParameter ArrayIndex;

        public override UniTask ExecuteAsync (AsyncToken asyncToken = default)
        {
            var value = CustomVariablesConfiguration.ParseVariableValue(VariableValue);

            if (Assigned(ArrayIndex))
            {
                var var = FsmVariables.GlobalVariables.FindFsmArray(VariableName);
                if (var is null)
                {
                    Err($"PlayMaker global array variable with name `{VariableName}` not found.");
                    return UniTask.CompletedTask;
                }
                var.Set(ArrayIndex, value);
            }
            else
            {
                var var = FsmVariables.GlobalVariables.FindVariable(VariableName);
                if (var is null)
                {
                    Err($"PlayMaker global variable with name `{VariableName}` not found.");
                    return UniTask.CompletedTask;
                }
                var.RawValue = value;
            }

            return UniTask.CompletedTask;
        }
    }
}
