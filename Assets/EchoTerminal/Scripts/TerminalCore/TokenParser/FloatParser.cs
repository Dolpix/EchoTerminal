public class FloatParser : ITokenParser
{
	public System.Type Type => typeof(float);

	public TokenState Parse(string raw)
	{
		if (float.TryParse(raw, out _))
		{
			return TokenState.Resolved;
		}

		return TokenState.Unresolved;
	}
}