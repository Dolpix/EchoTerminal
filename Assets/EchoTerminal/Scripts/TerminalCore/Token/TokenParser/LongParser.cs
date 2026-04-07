using System;

public class LongParser : ITokenParser
{
	public Type Type => typeof(long);

	public TokenState ParseTokenState(string raw, Type expectedType = null)
	{
		return long.TryParse(raw, out _) ? TokenState.Resolved : TokenState.Unresolved;
	}

	public object ParseValue(string raw, Type expectedType = null)
	{
		return long.Parse(raw);
	}
}