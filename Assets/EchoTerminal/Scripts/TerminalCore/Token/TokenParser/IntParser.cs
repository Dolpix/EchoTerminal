public class IntParser : ITokenParser
{
	public System.Type Type => typeof(int);

	public TokenState ParseTokenState(string raw, System.Type expectedType = null)
	{
		if (int.TryParse(raw, out _))
		{
			return TokenState.Resolved;
		}

		return TokenState.Unresolved;
	}

	public object ParseValue(string raw, System.Type expectedType = null)
	{
		return int.Parse(raw);
	}
}