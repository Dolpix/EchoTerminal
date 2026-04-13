using System;

public class BoolParser : ITokenParser
{
	public Type Type => typeof(bool);

	public TokenState ParseTokenState(string raw, Type expectedType = null)
	{
		if (raw.Length == 0)
		{
			return TokenState.Failed;
		}

		var lower = raw.ToLowerInvariant();

		if (lower is "true" or "false")
		{
			return TokenState.Completed;
		}

		if ((lower.Length < "true".Length && "true".StartsWith(lower)) ||
			(lower.Length < "false".Length && "false".StartsWith(lower)))
		{
			return TokenState.Partial;
		}

		return TokenState.Failed;
	}

	public object ParseValue(string raw, Type expectedType = null)
	{
		return bool.Parse(raw);
	}
}