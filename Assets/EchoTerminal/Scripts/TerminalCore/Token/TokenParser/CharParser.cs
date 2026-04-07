using System;

public class CharParser : ITokenParser
{
	public Type Type => typeof(char);

	public TokenState ParseTokenState(string raw, Type expectedType = null)
	{
		if (raw.Length == 0 || raw[0] != '\'')
		{
			return TokenState.Unresolved;
		}

		if (!raw.EndsWith("'") || raw.Length == 1)
		{
			return TokenState.Pending;
		}

		return raw.Length == 3 ? TokenState.Resolved : TokenState.Invalid;
	}

	public object ParseValue(string raw, Type expectedType = null)
	{
		return raw[1];
	}
}