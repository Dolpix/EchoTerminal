using System;

public class BoolParser : ITokenParser
{
	public Type Type => typeof(bool);

	public TokenState ParseTokenState(string raw, System.Type expectedType = null)
	{
		return raw is "true" or "false" ? TokenState.Resolved : TokenState.Unresolved;
	}

	public object ParseValue(string raw, System.Type expectedType = null)
	{
		return bool.Parse(raw);
	}
}