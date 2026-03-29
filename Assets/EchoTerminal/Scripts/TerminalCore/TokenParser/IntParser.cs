public class IntParser : ITokenParser
{
	public System.Type Type => typeof(int);

	public TokenState Parse(string raw)
	{
		if (int.TryParse(raw, out _))
		{
			return TokenState.Resolved;
		}

		return TokenState.Unresolved;
	}
}