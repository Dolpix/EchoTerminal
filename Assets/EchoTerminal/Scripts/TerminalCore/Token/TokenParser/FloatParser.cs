public class FloatParser : ITokenParser
{
	public System.Type Type => typeof(float);

	public TokenState ParseTokenState(string raw, System.Type expectedType = null)
	{
		if (float.TryParse(raw, out _))
		{
			return TokenState.Completed;
		}

		return TokenState.Failed;
	}

	public object ParseValue(string raw, System.Type expectedType = null)
	{
		return float.Parse(raw);
	}
}