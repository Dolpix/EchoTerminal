public class FloatParser : ITokenParser
{
	public string Name => "Float";

	public TokenState Parse(string raw, bool isFinalized)
	{
		if (float.TryParse(raw, out _))
		{
			return TokenState.Resolved;
		}

		return TokenState.Unresolved;
	}
}