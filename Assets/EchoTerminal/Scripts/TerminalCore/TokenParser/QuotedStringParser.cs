public class QuotedStringParser : ITokenParser
{
	public System.Type Type => typeof(string);

	public TokenState Parse(string raw)
	{
		if (raw.Length == 0 || raw[0] != '"')
		{
			return TokenState.Unresolved;
		}

		if (raw.Length >= 2 && raw[^1] == '"')
		{
			return TokenState.Resolved;
		}

		return TokenState.Pending;
	}
}