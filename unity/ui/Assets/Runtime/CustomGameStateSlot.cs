using System.Linq;
using Naninovel;
using Naninovel.UI;

public class CustomGameStateSlot : GameStateSlot
{
    protected override void SetTitleText (string value)
    {
        // Find last printed text.
        var lastText = GetLastPrintedText();
        // Append the text to the slot title in case it's not empty.
        if (!string.IsNullOrEmpty(lastText))
            value += $"\n{lastText}";
        base.SetTitleText(value);
    }

    private string GetLastPrintedText ()
    {
        // Find state of the first printer actor that is visible and has something printed.
        var printerState = State
            ?.GetState<ActorManager<ITextPrinterActor, TextPrinterState, TextPrinterMetadata, TextPrintersConfiguration>.GameState>()
            ?.ActorsMap.Values.FirstOrDefault(s => s.Visible && !string.IsNullOrWhiteSpace(s.Text));
        if (printerState is null) return null;
        // Prepend actor display name in case printed text has an author.
        if (string.IsNullOrEmpty(printerState.AuthorId)) return printerState.Text;
        else return $"{GetDisplayName(printerState.AuthorId)}: {printerState.Text}";
    }

    private string GetDisplayName (string authorId)
    {
        var characterManager = Engine.GetService<ICharacterManager>();
        return characterManager.GetDisplayName(authorId) ?? authorId;
    }
}
