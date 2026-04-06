using System;

public class BoolParser : ITokenParser
{
	public Type Type => typeof(bool);

	public TokenState ParseTokenState(string raw, Type expectedType = null)
	{
		if (raw.Length == 0)
		{
			return TokenState.Unresolved;
		}

		var lower = raw.ToLowerInvariant();

		if (lower is "true" or "false")
		{
			return TokenState.Resolved;
		}

		if ((lower.Length < "true".Length && "true".StartsWith(lower)) ||
			(lower.Length < "false".Length && "false".StartsWith(lower)))
		{
			return TokenState.Pending;
		}

		return TokenState.Unresolved;
	}

	public object ParseValue(string raw, Type expectedType = null)
	{
		return bool.Parse(raw);
	}
}