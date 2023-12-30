using HutongGames.PlayMaker;

namespace Naninovel.PlayMaker
{
    [ExpressionFunctions]
    public static class ExpressionFunctions
    {
        public static string GetPlayMakerGlobalVariable (string name)
        {
            var var = FsmVariables.GlobalVariables.FindVariable(name);
            return var?.RawValue?.ToString();
        }

        public static string GetPlayMakerGlobalArray (string name, int index)
        {
            var var = FsmVariables.GlobalVariables.FindFsmArray(name);
            return var?.Values?.GetValue(index)?.ToString();
        }
    }
}
