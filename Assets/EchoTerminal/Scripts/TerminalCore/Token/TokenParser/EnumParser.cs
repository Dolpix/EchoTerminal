using System;
using System.Linq;

public class EnumParser : ITokenParser
{
	public Type Type => typeof(Enum);

	public TokenState ParseTokenState(string raw, Type expectedType = null)
	{
		if (raw.Length == 0 || expectedType is not { IsEnum: true })
		{
			return TokenState.Unresolved;
		}

		var names = Enum.GetNames(expectedType);

		if (names.Any(n => string.Equals(n, raw, StringComparison.OrdinalIgnoreCase)))
		{
			return TokenState.Resolved;
		}

		var lower = raw.ToLowerInvariant();
		if (names.Any(n => n.Length > raw.Length && n.ToLowerInvariant().StartsWith(lower)))
		{
			return TokenState.Pending;
		}

		return TokenState.Unresolved;
	}

	public object ParseValue(string raw, Type expectedType = null)
	{
		return expectedType is not { IsEnum: true } ? null : Enum.Parse(expectedType, raw, true);
	}
}