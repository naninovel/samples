using Naninovel;
using Naninovel.UI;
using UnityEngine;

public class CustomChatMessage : ChatMessage
{
    public override string MessageText { get => originalMessage; set => SetMessageText(value); }

    [SerializeField] private StringUnityEvent onDateChanged = default;

    private string originalMessage;

    private void SetMessageText (string message)
    {
        originalMessage = message;
        ExtractDate(ref message, out var date);
        onDateChanged?.Invoke(date);
        base.MessageText = message;
    }

    private static void ExtractDate (ref string message, out string date)
    {
        date = null;
        if (string.IsNullOrEmpty(message) || message[0] != '(') return;
        var endIndex = message.IndexOf(')');
        if (endIndex <= 1) return;
        date = message.Substring(1, endIndex - 1);
        message = message.Substring(endIndex + 1).TrimStart();
    }
}
