using Naninovel;

[CommandAlias("hour")]
public class SetHour : Command
{
    [ParameterAlias(NamelessParameterAlias), RequiredParameter]
    public DecimalParameter Hour;
    public DecimalParameter Duration = 1;

    public override async UniTask ExecuteAsync (AsyncToken token = default)
    {
        await TimeOfDay.SetHourAsync(Hour, Duration, token);
    }
}
