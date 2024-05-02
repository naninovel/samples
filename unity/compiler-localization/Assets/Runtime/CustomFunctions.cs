using Naninovel;

public static class CustomFunctions
{
    [ExpressionFunction("тестовая_функция")]
    public static string ToUpper (string text)
    {
        return text.ToUpperInvariant();
    }
}
