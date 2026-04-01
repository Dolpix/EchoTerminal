public class FloatParser : ITokenParser
{
	public System.Type Type => typeof(float);

	public TokenState ParseTokenState(string raw)
	{
		if (float.TryParse(raw, out _))
		{
			return TokenState.Resolved;
		}

		return TokenState.Unresolved;
	}

	public object ParseValue(string raw)
	{
		return float.Parse(raw);
	}
}