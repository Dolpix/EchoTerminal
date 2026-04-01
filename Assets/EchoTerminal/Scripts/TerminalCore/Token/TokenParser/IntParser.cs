public class IntParser : ITokenParser
{
	public System.Type Type => typeof(int);

	public TokenState ParseTokenState(string raw)
	{
		if (int.TryParse(raw, out _))
		{
			return TokenState.Resolved;
		}

		return TokenState.Unresolved;
	}

	public object ParseValue(string raw)
	{
		return int.Parse(raw);
	}
}