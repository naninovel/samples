using Naninovel;
using Naninovel.Commands;
using Naninovel.Parsing;

public class CustomGenericLineParser : GenericTextLineParser
{
    private float printSpeed;

    protected override GenericTextScriptLine Parse (GenericLine lineModel)
    {
        // Reset current print speed.
        printSpeed = -1;

        // Try extract print speed from the model.
        if (lineModel.Prefix?.Author.Text.Contains("-") ?? false)
        {
            ParseUtils.TryInvariantFloat(lineModel.Prefix.Author.Text.GetAfter("-"), out printSpeed);
            // Remove print speed from the author ID.
            var author = lineModel.Prefix.Author.Text.GetBeforeLast("-");
            var prefix = new GenericPrefix(author, lineModel.Prefix.Appearance);
            return base.Parse(new GenericLine(prefix, lineModel.Content));
        }
        return base.Parse(lineModel);
    }

    protected override void AddCommand (Naninovel.Command command)
    {
        // When assigned, set the speed to all the print commands in the line.
        if (printSpeed > 0 && command is PrintText print)
            print.RevealSpeed = printSpeed / 100;
        base.AddCommand(command);
    }
}
