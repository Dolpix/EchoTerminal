public class IntParser : ITokenParser
{
	public string Name => "Int";

	public TokenState Parse(string raw, bool isFinalized)
	{
		if (int.TryParse(raw, out _))
		{
			return TokenState.Resolved;
		}

		return TokenState.Unresolved;
	}
}