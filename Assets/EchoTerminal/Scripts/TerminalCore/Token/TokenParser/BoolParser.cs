using System;

public class BoolParser : ITokenParser
{
	public Type Type => typeof(bool);

	public TokenState ParseTokenState(string raw, System.Type expectedType = null)
	{
		if (bool.TryParse(raw, out _))
		{
			return TokenState.Resolved;
		}

		return TokenState.Unresolved;
	}

	public object ParseValue(string raw, System.Type expectedType = null)
	{
		return bool.Parse(raw);
	}
}