public class QuotedStringParser : ITokenParser
{
	public string Name => "QuotedString";

	public TokenState Parse(string raw, bool isFinalized)
	{
		if (raw.Length == 0 || raw[0] != '"')
		{
			return TokenState.Unresolved;
		}

		if (raw.Length >= 2 && raw[^1] == '"')
		{
			return TokenState.Resolved;
		}

		return isFinalized ? TokenState.Invalid : TokenState.Pending;
	}
}