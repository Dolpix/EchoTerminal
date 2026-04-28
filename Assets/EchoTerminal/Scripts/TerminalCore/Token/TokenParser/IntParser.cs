public class IntParser : ITokenParser, IHintLabeler
{
	public System.Type Type => typeof(int);
	public string HintLabel => "0";

	public TokenState ParseTokenState(string raw, System.Type expectedType = null)
	{
		if (int.TryParse(raw, out _))
		{
			return TokenState.Completed;
		}

		return TokenState.Failed;
	}

	public object ParseValue(string raw, System.Type expectedType = null)
	{
		return int.Parse(raw);
	}
}