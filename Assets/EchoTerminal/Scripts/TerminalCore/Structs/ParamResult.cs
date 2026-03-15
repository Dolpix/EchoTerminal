namespace EchoTerminal
{
public struct ParamResult
{
	public CommandParam Expected { get; }
	public string Token { get; }
	public object Value { get; }
	public bool IsValid { get; }

	public ParamResult(CommandParam expected, string token, object value, bool isValid)
	{
		Expected = expected;
		Token = token;
		Value = value;
		IsValid = isValid;
	}
}
}