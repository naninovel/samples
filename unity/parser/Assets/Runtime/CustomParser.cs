using System.Collections.Generic;
using Naninovel;

public class CustomParser : ScriptParser
{
    // Use custom generic line parser.
    protected override GenericTextLineParser GenericTextLineParser { get; } = new CustomGenericLineParser();

    public override Script ParseText (string scriptName, string scriptText, ICollection<ScriptParseError> errors = null)
    {
        // Insert wait commands after `...`.
        scriptText = scriptText.Replace("...", "...[wait 1]");

        // Notice: this naive implementation is just for demo purpose.
        // In real project you'd rather modify generic text parser 
        // to make sure this only affect the printed text and/or use
        // regex for a more precise matching.

        return base.ParseText(scriptName, scriptText, errors);
    }
}
