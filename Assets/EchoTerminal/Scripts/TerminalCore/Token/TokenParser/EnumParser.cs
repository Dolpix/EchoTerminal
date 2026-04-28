using System;
using System.Linq;

public class EnumParser : ITokenParser, IHintLabeler
{
	public Type Type => typeof(Enum);
	public string HintLabel => "value";

	public TokenState ParseTokenState(string raw, Type expectedType = null)
	{
		if (raw.Length == 0 || expectedType is not { IsEnum: true })
		{
			return TokenState.Failed;
		}

		var names = Enum.GetNames(expectedType);

		if (names.Any(n => string.Equals(n, raw, StringComparison.OrdinalIgnoreCase)))
		{
			return TokenState.Completed;
		}

		var lower = raw.ToLowerInvariant();
		if (names.Any(n => n.Length > raw.Length && n.ToLowerInvariant().StartsWith(lower)))
		{
			return TokenState.Partial;
		}

		return TokenState.Failed;
	}

	public object ParseValue(string raw, Type expectedType = null)
	{
		return expectedType is not { IsEnum: true } ? null : Enum.Parse(expectedType, raw, true);
	}
}